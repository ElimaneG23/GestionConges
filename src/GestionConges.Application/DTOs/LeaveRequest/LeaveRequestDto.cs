namespace GestionConges.Application.DTOs.LeaveRequest;

public record CreateLeaveRequestDto(
    Guid LeaveTypeId,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason
);

public record ProcessLeaveRequestDto(
    bool Approve,
    string? ManagerComment
);

public record LeaveRequestDto(
    Guid Id,
    Guid UserId,
    string EmployeeName,
    Guid LeaveTypeId,
    string LeaveTypeName,
    DateTime StartDate,
    DateTime EndDate,
    decimal NumberOfDays,
    string? Reason,
    string Status,
    string? ManagerComment,
    string? ProcessedByName,
    DateTime? ProcessedAt,
    DateTime CreatedAt
);
