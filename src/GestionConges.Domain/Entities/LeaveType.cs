using GestionConges.Domain.Common;

namespace GestionConges.Domain.Entities;

/// <summary>
/// Type de congé configurable par entreprise (congés payés, RTT, maladie...).
/// </summary>
public class LeaveType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DefaultDaysPerYear { get; set; }
    public bool RequiresJustification { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
}
