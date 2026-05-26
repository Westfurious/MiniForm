using MiniForm.Dtos.Forms;

namespace MiniForm.Application.Interfaces;

public interface IFormService
{
    Task<List<FormResponse>> GetFormsAsync(CancellationToken cancellationToken = default);

    Task<FormResponse?> GetFormByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<FormResponse> CreateFormAsync(CreateFormRequest request, Guid createdByUserId, CancellationToken cancellationToken = default);
}
