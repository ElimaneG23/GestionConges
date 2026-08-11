using GestionConges.Domain.Enums;

namespace GestionConges.Application.DTOs.LeaveRequest
{
    public class LeaveRequestFilterDto
    {
        public LeaveRequestStatus? Status { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class LeaveRequestResponseDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public Guid LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal NumberOfDays { get; set; }
        public string? Reason { get; set; }
        public LeaveRequestStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public string? ProcessedByName { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ManagerComment { get; set; }
    }
}
