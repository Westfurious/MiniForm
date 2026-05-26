using MiniForm.Application.Interfaces;
using MiniForm.Dtos.Forms;
using MiniForm.Models;

namespace MiniForm.Infrastructure.Services;

public class FormService : IFormService
{
    private readonly MiniForm.Application.Interfaces.IFormRepository _formRepository;
    private readonly MiniForm.Application.Interfaces.IUnitOfWork _unitOfWork;

    public FormService(MiniForm.Application.Interfaces.IFormRepository formRepository, MiniForm.Application.Interfaces.IUnitOfWork unitOfWork)
    {
        _formRepository = formRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<FormResponse>> GetFormsAsync(CancellationToken cancellationToken = default)
    {
        var forms = await _formRepository.GetAllAsync(cancellationToken);
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

        var created = await _formRepository.AddAsync(form, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToResponse(created);
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
