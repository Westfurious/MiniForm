using MiniForm.Models;

namespace MiniForm.Application.Interfaces;

public interface IFormService
{
    Task<List<Form>> GetFormsAsync(CancellationToken cancellationToken = default);

    Task<Form> CreateFormAsync(Form form, CancellationToken cancellationToken = default);
}
