using GestionConges.API.Common;
using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Tenant;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionConges.API.Controllers;

/// <summary>
/// Paramétrage de la plateforme par l'Admin de l'entreprise : logo, couleurs (US Admin),
/// et règles RH (Sprint 9 - US9.1).
/// </summary>
[ApiController]
[Route("api/tenant-settings")]
[Authorize(Policy = "AdminOnly")]
public class TenantSettingsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUser;

    public TenantSettingsController(ITenantService tenantService, ICurrentUserService currentUser)
    {
        _tenantService = tenantService;
        _currentUser = currentUser;
    }

    /// <summary>L'Admin personnalise le branding de sa plateforme (logo, couleurs).</summary>
    [HttpPut("branding")]
    public async Task<IActionResult> UpdateBranding(UpdateTenantBrandingDto dto)
        => (await _tenantService.UpdateBrandingAsync(_currentUser.TenantId!.Value, dto)).ToActionResult(this);

    /// <summary>US9.1 - Configurer les règles RH (jours ouvrés, report de congés...).</summary>
    [HttpPut("hr-rules")]
    public async Task<IActionResult> UpdateSettings(UpdateTenantSettingsDto dto)
        => (await _tenantService.UpdateSettingsAsync(_currentUser.TenantId!.Value, dto)).ToActionResult(this);

    [HttpGet("hr-rules")]
    public async Task<IActionResult> GetSettings()
        => (await _tenantService.GetSettingsAsync(_currentUser.TenantId!.Value)).ToActionResult(this);
}
