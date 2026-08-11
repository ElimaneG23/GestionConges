using GestionConges.API.Common;
using GestionConges.Application.Common;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CurrentUserServiceContract = GestionConges.Application.Common.ICurrentUserService;

namespace GestionConges.API.Controllers;

/// <summary>Sprint 6 : un tableau de bord dédié par rôle.</summary>
[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly CurrentUserServiceContract _currentUser;

    public DashboardController(IDashboardService dashboardService, CurrentUserServiceContract currentUser)
    {
        _dashboardService = dashboardService;
        _currentUser = currentUser;
    }

    /// <summary>US6.1 - Dashboard employé (solde, demandes récentes).</summary>
    [HttpGet("employee")]
    public async Task<IActionResult> Employee()
        => (await _dashboardService.GetEmployeeDashboardAsync(_currentUser.UserId!.Value)).ToActionResult(this);

    /// <summary>US6.2 - Dashboard manager (demandes à traiter, équipe).</summary>
    [HttpGet("manager")]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> Manager()
        => (await _dashboardService.GetManagerDashboardAsync(_currentUser.UserId!.Value)).ToActionResult(this);

    /// <summary>US6.3 - Dashboard RH / Admin (vue globale entreprise).</summary>
    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Admin()
        => (await _dashboardService.GetAdminDashboardAsync(_currentUser.TenantId!.Value)).ToActionResult(this);

    /// <summary>Dashboard SuperAdmin (vue plateforme, tous tenants confondus).</summary>
    [HttpGet("super-admin")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> SuperAdmin()
        => (await _dashboardService.GetSuperAdminDashboardAsync()).ToActionResult(this);
}
