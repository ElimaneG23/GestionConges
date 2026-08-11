using GestionConges.Domain.Common;

namespace GestionConges.Domain.Entities;

/// <summary>
/// Solde de congés d'un employé pour un type de congé donné et une année donnée.
/// </summary>
public class LeaveBalance : TenantEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid LeaveTypeId { get; set; }
    public LeaveType? LeaveType { get; set; }

    public int Year { get; set; }
    public decimal TotalDays { get; set; }
    public decimal UsedDays { get; set; }
    public decimal RemainingDays => TotalDays - UsedDays;
}
