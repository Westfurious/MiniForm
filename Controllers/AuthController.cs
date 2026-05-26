using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniForm.Data;
using MiniForm.Dtos.Auth;
using MiniForm.Models;
using MiniForm.Services.Abstractions;

namespace MiniForm.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly MiniForm.Application.Interfaces.IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(MiniForm.Application.Interfaces.IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.RegisterAsync(request, cancellationToken);
            return Ok(_tokenService.CreateToken(user));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.ValidateCredentialsAsync(request, cancellationToken);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(_tokenService.CreateToken(user));
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
