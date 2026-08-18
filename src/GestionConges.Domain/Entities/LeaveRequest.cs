using GestionConges.Domain.Common;
using GestionConges.Domain.Enums;

namespace GestionConges.Domain.Entities;

/// <summary>
/// Demande de congé soumise par un employé et traitée par son manager.
/// </summary>
public class LeaveRequest : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid LeaveTypeId { get; set; }
    public LeaveType? LeaveType { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal NumberOfDays { get; set; }
    public string? Reason { get; set; }

    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;

    public Guid? ProcessedByUserId { get; set; }
    public User? ProcessedByUser { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ManagerComment { get; set; }
}
