using GestionConges.API.Common;
using GestionConges.Application.Common;
using GestionConges.Application.DTOs.LeaveType;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CurrentUserServiceContract = GestionConges.Application.Common.ICurrentUserService;

namespace GestionConges.API.Controllers;

/// <summary>Sprint 3 : types de congés configurables par entreprise.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveTypesController : ControllerBase
{
    private readonly ILeaveTypeService _leaveTypeService;
    private readonly CurrentUserServiceContract _currentUser;

    public LeaveTypesController(ILeaveTypeService leaveTypeService, CurrentUserServiceContract currentUser)
    {
        _leaveTypeService = leaveTypeService;
        _currentUser = currentUser;
    }

    /// <summary>US3.1 - Créer un type de congé.</summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(CreateLeaveTypeDto dto)
        => (await _leaveTypeService.CreateAsync(_currentUser.TenantId!.Value, dto)).ToActionResult(this);

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => (await _leaveTypeService.GetAllAsync(_currentUser.TenantId!.Value)).ToActionResult(this);

    /// <summary>US3.2 - Configurer le nombre de jours par défaut / justificatif requis.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, UpdateLeaveTypeDto dto)
        => (await _leaveTypeService.UpdateAsync(id, dto)).ToActionResult(this);

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
        => (await _leaveTypeService.DeleteAsync(id)).ToActionResult(this);
}
