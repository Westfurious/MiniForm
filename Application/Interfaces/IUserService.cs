using MiniForm.Dtos.Auth;
using MiniForm.Models;

namespace MiniForm.Application.Interfaces;

public interface IUserService
{
    Task<User> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<User?> ValidateCredentialsAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
