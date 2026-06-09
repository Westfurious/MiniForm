class ApiClient {
    static getToken() {
        return localStorage.getItem('accessToken');
    }

    static setToken(token) {
        localStorage.setItem('accessToken', token);
    }

    static clearToken() {
        localStorage.removeItem('accessToken');
    }

    static async request(endpoint, method = 'GET', body = null) {
        const headers = { 'Content-Type': 'application/json' };

        const token = this.getToken();
        
        if (token) headers['Authorization'] = `Bearer ${token}`;

        const config = { method, headers };
        
        if (body) config.body = JSON.stringify(body);

        const response = await fetch(`${endpoint}`, config);

        if (!response.ok) {
            if (response.status === 401) this.clearToken();
            const errorText = await response.text();
            console.error("BACKEND ERROR:", errorText);
            
            throw new Error(`API Error: ${response.status}`);
        }

        const text = await response.text();
        return text ? JSON.parse(text) : null;
    }

    static login(email, password) {
        return this.request('/api/Auth/login', 'POST', { email, password });
    }

    static register(email, password) {
        return this.request('/api/Auth/register', 'POST', { email, password });
    }

    static getForms(page = 1) {
        return this.request(`/api/Forms?page=${page}`);
    }

    static getForm(id) {
        return this.request(`/api/Forms/${id}`);
    }

    static createForm(formDto) {
        return this.request('/api/Forms', 'POST', formDto);
    }

    static submitFormResponses(formId, answersDto) {
        return this.request(`/api/Forms/${formId}/responses`, 'POST', answersDto);
    }

    static getAnalytics(formId) {
        return this.request(`/api/forms/${formId}/analytics`);
    }
}

export default ApiClient;