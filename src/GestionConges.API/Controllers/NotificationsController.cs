using GestionConges.API.Common;
using GestionConges.Application.Common;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionConges.API.Controllers;

/// <summary>Sprint 7 : notifications in-app générées automatiquement par le workflow métier.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUser;

    public NotificationsController(INotificationService notificationService, ICurrentUserService currentUser)
    {
        _notificationService = notificationService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine()
        => (await _notificationService.GetMyNotificationsAsync(_currentUser.UserId!.Value)).ToActionResult(this);

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
        => (await _notificationService.MarkAsReadAsync(id, _currentUser.UserId!.Value)).ToActionResult(this);
}
