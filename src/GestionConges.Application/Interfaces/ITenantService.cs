using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Tenant;

namespace GestionConges.Application.Interfaces;

/// <summary>
/// Réservé au SuperAdmin : création/gestion des entreprises (tenants) de la plateforme.
/// </summary>
public interface ITenantService
{
    Task<Result<TenantDto>> CreateTenantAsync(CreateTenantDto dto);
    Task<Result<List<TenantDto>>> GetAllTenantsAsync();
    Task<Result<TenantDto>> GetTenantByIdAsync(Guid id);
    Task<Result<bool>> ActivateDeactivateTenantAsync(Guid id, bool isActive);

    // Paramétrage réservé à l'Admin de l'entreprise (branding)
    Task<Result<TenantDto>> UpdateBrandingAsync(Guid tenantId, UpdateTenantBrandingDto dto);
    Task<Result<UpdateTenantSettingsDto>> UpdateSettingsAsync(Guid tenantId, UpdateTenantSettingsDto dto);
    Task<Result<UpdateTenantSettingsDto>> GetSettingsAsync(Guid tenantId);
}
