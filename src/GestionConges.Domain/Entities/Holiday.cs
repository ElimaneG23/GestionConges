using GestionConges.Domain.Common;

namespace GestionConges.Domain.Entities;

/// <summary>
/// Jour férié / chômé configuré par l'Admin (Sprint 9), utilisé pour exclure
/// les jours du calcul du nombre de jours de congés décomptés.
/// </summary>
public class Holiday : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsRecurringYearly { get; set; } = false;
}
