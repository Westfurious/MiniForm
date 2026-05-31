using MiniForm.Dtos.Auth;
using MiniForm.Models;

namespace MiniForm.Services.Abstractions;

public interface ITokenService
{
    AuthResponse CreateToken(User user);
}
