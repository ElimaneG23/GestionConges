using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Notification;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using GestionConges.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IAppDbContext _db;

    public NotificationService(IAppDbContext db) => _db = db;

    public async Task NotifyAsync(Guid tenantId, Guid userId, string title, string message,
        NotificationType type, Guid? relatedLeaveRequestId = null)
    {
        var notification = new Notification
        {
            TenantId = tenantId,
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RelatedLeaveRequestId = relatedLeaveRequestId,
            IsRead = false
        };
        await _db.Notifications.AddAsync(notification);
        await _db.SaveChangesAsync();
        // Remarque : l'envoi e-mail/push réel se brancherait ici (ex: IEmailSender),
        // l'enregistrement en base assure déjà la notification "in-app".
    }

    public async Task<Result<List<NotificationDto>>> GetMyNotificationsAsync(Guid userId)
    {
        var list = await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();

        return Result<List<NotificationDto>>.Ok(list.Select(n =>
            new NotificationDto(n.Id, n.Title, n.Message, n.Type.ToString(), n.IsRead, n.CreatedAt)).ToList());
    }

    public async Task<Result<bool>> MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);
        if (n is null) return Result<bool>.Fail("Notification introuvable.");
        n.IsRead = true;
        await _db.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }
}
