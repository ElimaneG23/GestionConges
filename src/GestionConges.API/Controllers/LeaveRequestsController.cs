using GestionConges.API.Common;
using GestionConges.Application.Common;
using GestionConges.Application.DTOs.LeaveRequest;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CurrentUserServiceContract = GestionConges.Application.Common.ICurrentUserService;

namespace GestionConges.API.Controllers;

/// <summary>
/// Sprint 4 (demandes) + Sprint 5 (validation manager) : cœur métier de l'application.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;
    private readonly CurrentUserServiceContract _currentUser;

    public LeaveRequestsController(ILeaveRequestService leaveRequestService, CurrentUserServiceContract currentUser)
    {
        _leaveRequestService = leaveRequestService;
        _currentUser = currentUser;
    }

    /// <summary>US4.1 - L'employé crée une demande de congé.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateLeaveRequestDto dto)
        => (await _leaveRequestService.CreateAsync(_currentUser.UserId!.Value, _currentUser.TenantId!.Value, dto))
            .ToActionResult(this);

    /// <summary>US4.2 - L'employé consulte ses propres demandes.</summary>
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
        => (await _leaveRequestService.GetMyRequestsAsync(_currentUser.UserId!.Value)).ToActionResult(this);

    /// <summary>US4.3 - Annuler une demande encore en attente.</summary>
    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
        => (await _leaveRequestService.CancelAsync(id, _currentUser.UserId!.Value)).ToActionResult(this);

    /// <summary>US5.1 - Le manager consulte les demandes de son équipe à traiter.</summary>
    [HttpGet("pending-for-me")]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> GetPendingForManager()
        => (await _leaveRequestService.GetPendingForManagerAsync(_currentUser.UserId!.Value)).ToActionResult(this);

    /// <summary>US5.2 / US5.3 - Le manager approuve ou refuse une demande.</summary>
    [HttpPatch("{id:guid}/process")]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> Process(Guid id, ProcessLeaveRequestDto dto)
        => (await _leaveRequestService.ProcessAsync(id, _currentUser.UserId!.Value, dto)).ToActionResult(this);
}
