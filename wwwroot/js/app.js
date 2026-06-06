import ApiClient from './api.js';

const views = [
    'view-auth',
    'view-dashboard',
    'view-create-form',
    'view-fill-form',
    'view-analytics'
];

function showView(viewId) {
    views.forEach(id => {
        document.getElementById(id).style.display = (id === viewId) ? 'block' : 'none';
    });
    document.getElementById('navbar').style.display = ApiClient.getToken() ? 'block' : 'none';
}

document.addEventListener('DOMContentLoaded', () => {
    setupEventListeners();

    const urlParams = new URLSearchParams(window.location.search);
    const sharedFormId = urlParams.get('formId');

    if (sharedFormId) window.openForm(sharedFormId);
    else if (ApiClient.getToken()) loadDashboard();
    else showView('view-auth');
});

function setupEventListeners() {
    document.getElementById('nav-logout').addEventListener('click', () => {
        ApiClient.clearToken();
        showView('view-auth');
    });
    document.getElementById('nav-dashboard').addEventListener('click', loadDashboard);
    document.getElementById('nav-create').addEventListener('click', initCreateForm);

    document.getElementById('auth-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        await handleAuth(true);
    });
    document.getElementById('btn-register').addEventListener('click', async () => {
        await handleAuth(false);
    });

    document.getElementById('btn-add-question').addEventListener('click', addQuestionToBuilder);
    document.getElementById('btn-save-form').addEventListener('click', saveNewForm);
    document.getElementById('fill-form-element').addEventListener('submit', submitFormAnswers);
}

async function handleAuth(isLogin) {
    const email = document.getElementById('auth-email').value;
    const pass = document.getElementById('auth-password').value;
    const errorEl = document.getElementById('auth-error');
    errorEl.textContent = '';

    try {
        const response = isLogin 
            ? await ApiClient.login(email, pass) 
            : await ApiClient.register(email, pass);
        
        ApiClient.setToken(response.accessToken);
        loadDashboard();
    } catch (err) {
        errorEl.textContent = 'Ошибка авторизации.';
    }
}

async function loadDashboard() {
    showView('view-dashboard');
    const listEl = document.getElementById('forms-list');
    listEl.innerHTML = 'Загрузка...';

    try {
        const forms = await ApiClient.getForms(1);
        listEl.innerHTML = '';
        
        forms.forEach(form => {
            const li = document.createElement('li');
            li.innerHTML = `
                <strong>${form.title || 'Без названия'}</strong> 
                <div style="display: flex; gap: 10px;">
                    <button onclick="window.openForm('${form.id}')">Пройти</button>
                    <button onclick="window.viewAnalytics('${form.id}')">Аналитика</button>
                    <button onclick="window.copyShareLink('${form.id}')">Поделиться</button>
                </div>
            `;
            listEl.appendChild(li);
        });
    } catch (err) {
        listEl.innerHTML = 'Ошибка загрузки форм';
    }
}
window.loadDashboard = loadDashboard;
// Новая форма
let builderState = [];

function initCreateForm() {
    showView('view-create-form');
    document.getElementById('create-title').value = '';
    document.getElementById('create-desc').value = '';
    document.getElementById('create-anonymous').checked = false;
    builderState = [];
    renderBuilder();
}

function addQuestionToBuilder() {
    const type = parseInt(document.getElementById('question-type-select').value);
    
    const newQuestion = {
        title: '',
        isRequired: false,
        type: type
    };

    if (type === 1 || type === 2) {
        newQuestion.options = ['Вариант 1']; 
    }

    builderState.push(newQuestion);
    renderBuilder();
}

function renderBuilder() {
    const container = document.getElementById('questions-builder');
    container.innerHTML = '';

    builderState.forEach((q, qIndex) => {
        const div = document.createElement('div');
        div.style.border = '1px solid black';
        div.style.padding = '10px';
        div.style.marginBottom = '10px';

        const typeName = q.type === 0 ? 'Текст' : q.type === 1 ? 'Один вариант' : 'Несколько вариантов';

        let html = `
            <div><b>Вопрос ${qIndex + 1} [${typeName}]</b> 
                 <button onclick="window.removeQuestion(${qIndex})">Удалить вопрос</button></div>
            <input type="text" placeholder="Текст вопроса" value="${q.title}" oninput="window.updateQTitle(${qIndex}, this.value)">
            <label><input type="checkbox" ${q.isRequired ? 'checked' : ''} onchange="window.updateQReq(${qIndex}, this.checked)"> Обязательный</label>
        `;

        if (q.type === 1 || q.type === 2) {
            html += `<ul>`;
            q.options.forEach((opt, optIndex) => {
                html += `
                    <li>
                        <input type="text" value="${opt}" oninput="window.updateQOption(${qIndex}, ${optIndex}, this.value)">
                        <button onclick="window.removeQOption(${qIndex}, ${optIndex})">x</button>
                    </li>`;
            });
            html += `</ul><button onclick="window.addQOption(${qIndex})">+ Добавить вариант</button>`;
        }

        div.innerHTML = html;
        container.appendChild(div);
    });
}

window.updateQTitle = (qIdx, val) => builderState[qIdx].title = val;
window.updateQReq = (qIdx, val) => builderState[qIdx].isRequired = val;
window.updateQOption = (qIdx, optIdx, val) => builderState[qIdx].options[optIdx] = val;
window.addQOption = (qIdx) => { builderState[qIdx].options.push(''); renderBuilder(); };
window.removeQOption = (qIdx, optIdx) => { builderState[qIdx].options.splice(optIdx, 1); renderBuilder(); };
window.removeQuestion = (qIdx) => { builderState.splice(qIdx, 1); renderBuilder(); };
window.copyShareLink = function(formId) {
    const baseUrl = window.location.origin + window.location.pathname;
    const shareUrl = `${baseUrl}?formId=${formId}`;
    navigator.clipboard.writeText(shareUrl).then(() => {
        alert('Ссылка скопирована в буфер обмена:\n' + shareUrl);
    }).catch(err => {
        alert('Не удалось скопировать ссылку.');
        console.error(err);
    });
}

async function saveNewForm() {
    const title = document.getElementById('create-title').value;
    if (!title) return alert('Введите название формы');

    const requestPayload = {
        title: title,
        description: document.getElementById('create-desc').value,
        isAnonymous: document.getElementById('create-anonymous').checked, // Отправляем только это поле
        questions: builderState.map(q => {
            const resultObj = {
                title: q.title,
                isRequired: q.isRequired,
                type: q.type
            };
            if (q.type === 1 || q.type === 2) {
                // фильтр на пустые строки
                resultObj.options = q.options.filter(opt => opt.trim() !== '');
            }
            return resultObj;
        })
    };

    try {
        await ApiClient.createForm(requestPayload);
        alert('Форма успешно создана!');
        loadDashboard();
    } catch (err) {
        alert('Ошибка при создании формы :`(');
        console.error(err);
    }
}
// Прохождение формы

let currentFillingForm = null;

window.openForm = async function(formId) {
    showView('view-fill-form');
    const container = document.getElementById('fill-questions');
    container.innerHTML = 'Загрузка...';

    try {
        currentFillingForm = await ApiClient.getForm(formId);
        document.getElementById('fill-title').textContent = currentFillingForm.title;
        document.getElementById('fill-desc').textContent = currentFillingForm.description;
        
        container.innerHTML = '';
        
        if (currentFillingForm.questions) {
            currentFillingForm.questions.forEach(q => {
                const div = document.createElement('div');
                div.style.marginBottom = '15px';
                let html = `<p><b>${q.title}</b> ${q.isRequired ? '<span style="color:red">*</span>' : ''}</p>`;

                if (q.type === 0) {
                    // Текстовый вопрос
                    html += `<input type="text" id="ans-${q.id}" ${q.isRequired ? 'required' : ''}>`;
                } 
                else if (q.type === 1) {
                    // Radio
                    q.options.forEach(opt => {
                        html += `<label><input type="radio" name="ans-${q.id}" value="${opt.id}" ${q.isRequired ? 'required' : ''}> ${opt.text}</label><br>`;
                    });
                } 
                else if (q.type === 2) {
                    // Checkbox
                    q.options.forEach(opt => {
                        html += `<label><input type="checkbox" name="ans-${q.id}" value="${opt.id}"> ${opt.text}</label><br>`;
                    });
                }
                
                div.innerHTML = html;
                container.appendChild(div);
            });
        }
    } catch (err) {
        container.innerHTML = 'Ошибка загрузки формы';
    }
}

async function submitFormAnswers(e) {
    e.preventDefault();
    const request = { answers: [] };
    let hasValidationError = false;

    currentFillingForm.questions.forEach(q => {
        let isAnswered = false;

        if (q.type === 0) { 
            // 0
            const val = document.getElementById(`ans-${q.id}`).value.trim();
            if (val !== "") {
                request.answers.push({ questionId: q.id, answerText: val, selectedOptionId: null });
                isAnswered = true;
            }
        } 
        else if (q.type === 1) { 
            // 1
            const selected = document.querySelector(`input[name="ans-${q.id}"]:checked`);
            if (selected) {
                request.answers.push({ questionId: q.id, answerText: "", selectedOptionId: selected.value });
                isAnswered = true;
            }
        } 
        else if (q.type === 2) { 
            // 2
            const checkedBoxes = document.querySelectorAll(`input[name="ans-${q.id}"]:checked`);
            if (checkedBoxes.length > 0) {
                checkedBoxes.forEach(box => {
                    request.answers.push({ questionId: q.id, answerText: "", selectedOptionId: box.value });
                });
                isAnswered = true;
            }
        }

        if (q.isRequired && !isAnswered) {
            hasValidationError = true;
            alert(`Вопрос "${q.title}" обязателен для заполнения`);
        }
    });

    if (hasValidationError) return;

    try {
        console.log("sending payload on backend ", JSON.stringify(request, null, 2));
        await ApiClient.submitFormResponses(currentFillingForm.id, request);
        alert('Ответы отправлены');
        
        if (ApiClient.getToken()) {
            window.history.replaceState({}, document.title, window.location.pathname);
            loadDashboard();
        } else {
            document.getElementById('view-fill-form').innerHTML = '<h2>Спасибо! Ваши ответы приняты.</h2>';
        }

    } catch (err) {
        alert('Ошибка 400. Данные отклонены бэкендом');
        console.error('API ERROR:', err);
    }
}
window.viewAnalytics = async function(formId) {
    try {
        const data = await ApiClient.getAnalytics(formId);
        showView('view-analytics');
        
        const totalSubmissions = data.totalSubmissions || 0;
        document.getElementById('analytics-total').textContent = totalSubmissions;

        const container = document.getElementById('analytics-content');
        container.innerHTML = '';

        if (!data.questions || data.questions.length === 0) {
            container.innerHTML = '<p>Пока нет данных для отображения.</p>';
            return;
        }

        data.questions.forEach(q => {
            const emp = 'Пусто';
            const block = document.createElement('div');
            block.className = 'analytics-q-block';

            const title = document.createElement('div');
            title.className = 'analytics-q-title';
            title.textContent = q.title || emp;
            block.appendChild(title);

            let qTotal = 0;
            if (typeof q.totalAnswers === 'number') {
                qTotal = q.totalAnswers;
            } else if (Array.isArray(q.options) && q.options.length > 0) {
                qTotal = q.options.reduce((s, o) => s + (o.count || 0), 0);
            } else if (Array.isArray(q.textAnswers)) {
                qTotal = q.textAnswers.length;
            }

            if (q.type === 0) {
                const ul = document.createElement('ul');
                ul.className = 'text-answers';

                const answers = Array.isArray(q.textAnswers) ? q.textAnswers : [];
                if (answers.length === 0) {
                    ul.innerHTML = '<li>Нет ответов =(</li>';
                } else {
                    answers.forEach(a => {
                        const li = document.createElement('li');
                        
                        if (a && typeof a === 'object' && 'answerText' in a) {
                            li.textContent = a.answerText || emp;
                        } else if (typeof a === 'string') {
                            li.textContent = a || emp;
                        } else {
                            li.textContent = emp;
                        }
                        ul.appendChild(li);
                    });
                }
                block.appendChild(ul);
            } else {
                const options = Array.isArray(q.options) ? q.options : [];

                if (options.length === 0) {
                    const p = document.createElement('p');
                    p.textContent = 'Нет вариантов ответа';
                    block.appendChild(p);
                } else {
                    options.forEach(opt => {
                        const count = Number(opt.count || 0);
                        const baseTotal = qTotal > 0 ? qTotal : totalSubmissions;
                        const percent = baseTotal > 0 ? Math.round((count / baseTotal) * 100) : 0;

                        const barWrapper = document.createElement('div');
                        barWrapper.className = 'bar-wrapper';
                        barWrapper.innerHTML = `
                            <div class="bar-stats">
                                <span>${(opt.text != null ? opt.text : '—')}</span>
                                <span>${count} ответов (${percent}%)</span>
                            </div>
                            <div class="bar-container">
                                <div class="bar-fill" style="width: ${percent}%;"></div>
                            </div>
                        `;
                        block.appendChild(barWrapper);
                    });
                }
            }
            container.appendChild(block);
        });

    } catch (err) {
        alert('Ошибка загрузки аналитики');
        console.error(err);
    }
}