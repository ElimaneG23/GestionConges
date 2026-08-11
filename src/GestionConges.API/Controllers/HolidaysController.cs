using GestionConges.API.Common;
using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Holiday;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CurrentUserServiceContract = GestionConges.Application.Common.ICurrentUserService;

namespace GestionConges.API.Controllers;

/// <summary>Sprint 9 (US9.2) : jours fériés de l'entreprise, utilisés aussi par le calendrier (Sprint 8).</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HolidaysController : ControllerBase
{
    private readonly IHolidayService _holidayService;
    private readonly CurrentUserServiceContract _currentUser;

    public HolidaysController(IHolidayService holidayService, CurrentUserServiceContract currentUser)
    {
        _holidayService = holidayService;
        _currentUser = currentUser;
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(CreateHolidayDto dto)
        => (await _holidayService.CreateAsync(_currentUser.TenantId!.Value, dto)).ToActionResult(this);

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => (await _holidayService.GetAllAsync(_currentUser.TenantId!.Value)).ToActionResult(this);

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
        => (await _holidayService.DeleteAsync(id)).ToActionResult(this);
}
