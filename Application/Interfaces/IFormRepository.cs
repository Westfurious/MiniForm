using MiniForm.Models;

namespace MiniForm.Application.Interfaces;

public interface IFormRepository
{
    Task<Form?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Form> AddAsync(Form form, CancellationToken cancellationToken = default);

    Task<List<Form>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
}
