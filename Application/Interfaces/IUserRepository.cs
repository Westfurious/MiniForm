using MiniForm.Models;

namespace MiniForm.Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
