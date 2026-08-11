namespace GestionConges.Application.DTOs.LeaveType;

public record CreateLeaveTypeDto(
    string Name,
    string? Description,
    int DefaultDaysPerYear,
    bool RequiresJustification
);

public record UpdateLeaveTypeDto(
    string Name,
    string? Description,
    int DefaultDaysPerYear,
    bool RequiresJustification,
    bool IsActive
);

public record LeaveTypeDto(
    Guid Id,
    string Name,
    string? Description,
    int DefaultDaysPerYear,
    bool RequiresJustification,
    bool IsActive
);
