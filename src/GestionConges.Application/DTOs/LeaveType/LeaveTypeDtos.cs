namespace GestionConges.Application.DTOs.LeaveType
{
    public class LeaveTypeResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ColorCode { get; set; } = string.Empty;
        public decimal DefaultDaysPerYear { get; set; }
        public bool RequiresApproval { get; set; }
        public bool IsActive { get; set; }
    }
}
