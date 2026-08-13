using GestionConges.Domain.Enums;

namespace GestionConges.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
        public UserInfoDto User { get; set; }
        public TenantInfoDto Tenant { get; set; }
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
        public decimal RemainingLeaveDays { get; set; }
    }

    public class TenantInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
    }

    public class TenantRegistrationResponseDto
    {
        public Guid TenantId { get; set; }
        public string Subdomain { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public Guid AdminId { get; set; }
        public string AdminEmail { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
