using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Notification;
using GestionConges.Domain.Enums;

namespace GestionConges.Application.Interfaces;

public interface INotificationService
{
    Task NotifyAsync(Guid tenantId, Guid userId, string title, string message, NotificationType type, Guid? relatedLeaveRequestId = null);
    Task<Result<List<NotificationDto>>> GetMyNotificationsAsync(Guid userId);
    Task<Result<bool>> MarkAsReadAsync(Guid notificationId, Guid userId);
}
