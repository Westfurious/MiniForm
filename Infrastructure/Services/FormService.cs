using Microsoft.EntityFrameworkCore;
using MiniForm.Application.Interfaces;
using MiniForm.Data;
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

    public Task<List<Form>> GetFormsAsync(CancellationToken cancellationToken = default)
    {
        return _formRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Form> CreateFormAsync(Form form, CancellationToken cancellationToken = default)
    {
        var created = await _formRepository.AddAsync(form, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return created;
    }
}
