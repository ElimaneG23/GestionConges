using GestionConges.Domain.Common;
using GestionConges.Domain.Enums;

namespace GestionConges.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.General;
    public bool IsRead { get; set; } = false;

    public Guid? RelatedLeaveRequestId { get; set; }
}
