using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Tenant;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using GestionConges.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;

/// <summary>
/// Actions réservées au SuperAdmin (création de la plateforme / des entreprises)
/// et paramétrage confié ensuite à l'Admin de chaque entreprise (branding, règles RH).
/// </summary>
public class TenantService : ITenantService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public TenantService(IAppDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<TenantDto>> CreateTenantAsync(CreateTenantDto dto)
    {
        var subdomain = dto.Subdomain.Trim().ToLower();
        if (await _db.Tenants.AnyAsync(t => t.Subdomain == subdomain))
            return Result<TenantDto>.Fail("Ce sous-domaine est déjà utilisé.");

        if (await _db.Users.AnyAsync(u => u.Email.ToLower() == dto.AdminEmail.ToLower()))
            return Result<TenantDto>.Fail("Cet email administrateur est déjà utilisé.");

        var tenant = new Tenant
        {
            Name = dto.Name,
            Subdomain = subdomain,
            IsActive = true
        };
        await _db.Tenants.AddAsync(tenant);

        var settings = new TenantSettings { TenantId = tenant.Id, Tenant = tenant };
        await _db.TenantSettings.AddAsync(settings);

        var admin = new User
        {
            TenantId = tenant.Id,
            Tenant = tenant,
            FirstName = dto.AdminFirstName,
            LastName = dto.AdminLastName,
            Email = dto.AdminEmail,
            PasswordHash = _passwordHasher.Hash(dto.AdminPassword),
            Role = UserRole.Admin,
            IsActive = true
        };
        await _db.Users.AddAsync(admin);

        await _db.SaveChangesAsync();

        return Result<TenantDto>.Ok(ToDto(tenant, 1));
    }

    public async Task<Result<List<TenantDto>>> GetAllTenantsAsync()
    {
        var tenants = await _db.Tenants.ToListAsync();
        var result = new List<TenantDto>();
        foreach (var t in tenants)
        {
            var count = await _db.Users.CountAsync(u => u.TenantId == t.Id);
            result.Add(ToDto(t, count));
        }
        return Result<List<TenantDto>>.Ok(result);
    }

    public async Task<Result<TenantDto>> GetTenantByIdAsync(Guid id)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id);
        if (tenant is null) return Result<TenantDto>.Fail("Entreprise introuvable.");
        var count = await _db.Users.CountAsync(u => u.TenantId == id);
        return Result<TenantDto>.Ok(ToDto(tenant, count));
    }

    public async Task<Result<bool>> ActivateDeactivateTenantAsync(Guid id, bool isActive)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id);
        if (tenant is null) return Result<bool>.Fail("Entreprise introuvable.");
        tenant.IsActive = isActive;
        tenant.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }

    public async Task<Result<TenantDto>> UpdateBrandingAsync(Guid tenantId, UpdateTenantBrandingDto dto)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        if (tenant is null) return Result<TenantDto>.Fail("Entreprise introuvable.");

        tenant.LogoUrl = dto.LogoUrl;
        tenant.PrimaryColor = dto.PrimaryColor;
        tenant.SecondaryColor = dto.SecondaryColor;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        var count = await _db.Users.CountAsync(u => u.TenantId == tenantId);
        return Result<TenantDto>.Ok(ToDto(tenant, count));
    }

    public async Task<Result<UpdateTenantSettingsDto>> UpdateSettingsAsync(Guid tenantId, UpdateTenantSettingsDto dto)
    {
        var settings = await _db.TenantSettings.FirstOrDefaultAsync(s => s.TenantId == tenantId);
        if (settings is null) return Result<UpdateTenantSettingsDto>.Fail("Paramètres introuvables.");

        settings.WorkingDaysPerWeek = dto.WorkingDaysPerWeek;
        settings.CarryOverAllowed = dto.CarryOverAllowed;
        settings.MaxCarryOverDays = dto.MaxCarryOverDays;
        settings.AnnualLeaveDefaultDays = dto.AnnualLeaveDefaultDays;
        settings.RequireManagerApproval = dto.RequireManagerApproval;
        settings.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Result<UpdateTenantSettingsDto>.Ok(dto);
    }

    public async Task<Result<UpdateTenantSettingsDto>> GetSettingsAsync(Guid tenantId)
    {
        var s = await _db.TenantSettings.FirstOrDefaultAsync(x => x.TenantId == tenantId);
        if (s is null) return Result<UpdateTenantSettingsDto>.Fail("Paramètres introuvables.");
        return Result<UpdateTenantSettingsDto>.Ok(new UpdateTenantSettingsDto(
            s.WorkingDaysPerWeek, s.CarryOverAllowed, s.MaxCarryOverDays,
            s.AnnualLeaveDefaultDays, s.RequireManagerApproval));
    }

    private static TenantDto ToDto(Tenant t, int employeeCount) => new(
        t.Id, t.Name, t.Subdomain, t.LogoUrl, t.PrimaryColor, t.SecondaryColor,
        t.IsActive, t.CreatedAt, employeeCount);
}
