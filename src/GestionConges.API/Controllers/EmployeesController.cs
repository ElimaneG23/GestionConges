using GestionConges.API.Common;
using GestionConges.Application.Common;
using GestionConges.Application.DTOs.User;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionConges.API.Controllers;

/// <summary>Sprint 2 : gestion des employés/managers d'une entreprise (réservé à l'Admin).</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;

    public EmployeesController(IUserService userService, ICurrentUserService currentUser)
    {
        _userService = userService;
        _currentUser = currentUser;
    }

    /// <summary>US2.1 - Créer une fiche employé (ou manager).</summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(CreateEmployeeDto dto)
        => (await _userService.CreateEmployeeAsync(_currentUser.TenantId!.Value, dto)).ToActionResult(this);

    /// <summary>US2.2 - Lister et filtrer les employés (recherche + filtre par rôle).</summary>
    [HttpGet]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? role)
        => (await _userService.GetEmployeesAsync(_currentUser.TenantId!.Value, search, role)).ToActionResult(this);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => (await _userService.GetByIdAsync(id)).ToActionResult(this);

    /// <summary>US2.3 - Modifier un employé.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, UpdateEmployeeDto dto)
        => (await _userService.UpdateEmployeeAsync(id, dto)).ToActionResult(this);

    /// <summary>US1.3 - Désactivation utilisateur.</summary>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Deactivate(Guid id)
        => (await _userService.DeactivateAsync(id)).ToActionResult(this);
}
