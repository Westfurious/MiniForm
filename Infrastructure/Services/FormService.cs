using MiniForm.Application.Interfaces;
using MiniForm.Dtos.Forms;
using MiniForm.Models;

namespace MiniForm.Infrastructure.Services;

public class FormService : IFormService
{
    private readonly MiniForm.Application.Interfaces.IFormRepository _formRepository;
    private readonly MiniForm.Application.Interfaces.IUnitOfWork _unitOfWork;
    private readonly MiniForm.Application.Interfaces.ISubmissionRepository _submissionRepository;

    public FormService(MiniForm.Application.Interfaces.IFormRepository formRepository, MiniForm.Application.Interfaces.IUnitOfWork unitOfWork, MiniForm.Application.Interfaces.ISubmissionRepository submissionRepository)
    {
        _formRepository = formRepository;
        _unitOfWork = unitOfWork;
        _submissionRepository = submissionRepository;
    }

    public async Task<List<FormResponse>> GetFormsForUserAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var forms = await _formRepository.GetByUserAsync(userId, page, pageSize, cancellationToken);
        return forms.Select(MapToResponse).ToList();
    }

    public async Task<FormResponse?> GetFormByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var form = await _formRepository.GetByIdAsync(id, cancellationToken);
        if (form is null)
        {
            return null;
        }

        return MapToResponse(form);
    }

    public async Task SubmitFormAsync(Guid formId, SubmitFormRequest request, CancellationToken cancellationToken = default)
    {
        var form = await _formRepository.GetByIdAsync(formId, cancellationToken);
        if (form is null) throw new InvalidOperationException("Form not found.");

        // Validate required questions
        var requiredQuestionIds = form.Questions.Where(q => q.IsRequired).Select(q => q.Id).ToHashSet();
        var providedIds = request.Answers.Select(a => a.QuestionId).ToHashSet();

        var missing = requiredQuestionIds.Except(providedIds).ToList();
        if (missing.Any())
        {
            throw new InvalidOperationException($"Missing answers for required questions: {string.Join(',', missing)}");
        }

        // Build submission
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            FormId = formId,
            SubmittedAt = DateTime.UtcNow,
            Answers = request.Answers.Select(a => new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = a.QuestionId,
                AnswerText = string.IsNullOrWhiteSpace(a.AnswerText) ? null : a.AnswerText
            }).ToList()
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _submissionRepository.AddAsync(submission, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<FormResponse> CreateFormAsync(CreateFormRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
    {
        var form = new Form
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Deadline = request.Deadline,
            IsPublic = request.IsPublic,
            CreatedByUserId = createdByUserId,
            Questions = request.Questions.Select(question => new Question
            {
                Id = Guid.NewGuid(),
                Title = question.Title,
                IsRequired = question.IsRequired
            }).ToList()
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var created = await _formRepository.AddAsync(form, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return MapToResponse(created);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static FormResponse MapToResponse(Form form)
    {
        return new FormResponse
        {
            Id = form.Id,
            Title = form.Title,
            Description = form.Description,
            CreatedAt = form.CreatedAt,
            Deadline = form.Deadline,
            IsPublic = form.IsPublic,
            CreatedByUserId = form.CreatedByUserId,
            Questions = form.Questions.Select(question => new QuestionResponse
            {
                Id = question.Id,
                Title = question.Title,
                IsRequired = question.IsRequired
            }).ToList()
        };
    }
}
