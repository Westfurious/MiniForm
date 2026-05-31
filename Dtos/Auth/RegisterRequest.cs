using System.ComponentModel.DataAnnotations;

namespace MiniForm.Dtos.Auth;

public sealed record RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; init; } = string.Empty;
}
