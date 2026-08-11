using GestionConges.Domain.Enums;

namespace GestionConges.Application.DTOs.Notification
{
    public class NotificationResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
