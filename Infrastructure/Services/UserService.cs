using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniForm.Application.Interfaces;
using MiniForm.Data;
using MiniForm.Dtos.Auth;
using MiniForm.Models;

namespace MiniForm.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly MiniForm.Application.Interfaces.IUserRepository _userRepository;
    private readonly MiniForm.Application.Interfaces.IUnitOfWork _unitOfWork;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UserService(MiniForm.Application.Interfaces.IUserRepository userRepository, MiniForm.Application.Interfaces.IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        if (exists) throw new InvalidOperationException("User with this email already exists.");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return user;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<User?> ValidateCredentialsAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null) return null;
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        return result == PasswordVerificationResult.Success ? user : null;
    }
}
