namespace GestionConges.Application.DTOs.Notification;

public record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    string Type,
    bool IsRead,
    DateTime CreatedAt
);
