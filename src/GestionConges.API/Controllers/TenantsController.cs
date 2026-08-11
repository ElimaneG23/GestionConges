using GestionConges.API.Common;
using GestionConges.Application.DTOs.Tenant;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionConges.API.Controllers;

/// <summary>
/// Réservé au SuperAdmin : création et administration des entreprises (tenants)
/// de la plateforme SaaS.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SuperAdminOnly")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    public TenantsController(ITenantService tenantService) => _tenantService = tenantService;

    /// <summary>Le SuperAdmin crée une nouvelle entreprise cliente + son compte Admin.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateTenantDto dto)
        => (await _tenantService.CreateTenantAsync(dto)).ToActionResult(this);

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => (await _tenantService.GetAllTenantsAsync()).ToActionResult(this);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => (await _tenantService.GetTenantByIdAsync(id)).ToActionResult(this);

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
        => (await _tenantService.ActivateDeactivateTenantAsync(id, true)).ToActionResult(this);

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
        => (await _tenantService.ActivateDeactivateTenantAsync(id, false)).ToActionResult(this);
}
