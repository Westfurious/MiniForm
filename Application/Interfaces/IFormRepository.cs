using MiniForm.Models;

namespace MiniForm.Application.Interfaces;

public interface IFormRepository
{
    Task<List<Form>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Form> AddAsync(Form form, CancellationToken cancellationToken = default);
}
