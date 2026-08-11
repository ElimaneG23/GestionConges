namespace GestionConges.Application.DTOs.Holiday;

public record CreateHolidayDto(string Name, DateTime Date, bool IsRecurringYearly);

public record HolidayDto(Guid Id, string Name, DateTime Date, bool IsRecurringYearly);
