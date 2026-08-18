using GestionConges.Domain.Common;

namespace GestionConges.Domain.Entities;

/// <summary>
/// Règles RH paramétrables par l'Admin d'une entreprise (Sprint 9).
/// </summary>
public class TenantSettings : BaseEntity
{
    public int WorkingDaysPerWeek { get; set; } = 5;
    public bool CarryOverAllowed { get; set; } = false;
    public int MaxCarryOverDays { get; set; } = 0;
    public int AnnualLeaveDefaultDays { get; set; } = 30;
    public bool RequireManagerApproval { get; set; } = true;
}
