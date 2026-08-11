namespace GestionConges.Application.DTOs.Tenant;

public record CreateTenantDto(
    string Name,
    string Subdomain,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string AdminPassword
);

public record UpdateTenantBrandingDto(
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor
);

public record TenantDto(
    Guid Id,
    string Name,
    string Subdomain,
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor,
    bool IsActive,
    DateTime CreatedAt,
    int EmployeeCount
);

public record UpdateTenantSettingsDto(
    int WorkingDaysPerWeek,
    bool CarryOverAllowed,
    int MaxCarryOverDays,
    int AnnualLeaveDefaultDays,
    bool RequireManagerApproval
);
