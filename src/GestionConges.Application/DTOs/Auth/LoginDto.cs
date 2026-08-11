namespace GestionConges.Application.DTOs.Auth;

public record LoginRequestDto(string Email, string Password);

public record LoginResponseDto(
    string Token,
    DateTime ExpiresAt,
    Guid UserId,
    string FullName,
    string Role,
    Guid? TenantId,
    string? TenantName
);
