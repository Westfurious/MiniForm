namespace MiniForm.Dtos.Auth;

public sealed record AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public string TokenType { get; init; } = "Bearer";

    public DateTime ExpiresAtUtc { get; init; }

    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;
}
