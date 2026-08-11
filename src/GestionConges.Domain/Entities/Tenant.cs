using GestionConges.Domain.Common;

namespace GestionConges.Domain.Entities;

/// <summary>
/// Une entreprise cliente de la plateforme SaaS (créée par le SuperAdmin,
/// paramétrée ensuite par l'Admin : logo, couleurs, sous-domaine...).
/// </summary>
public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;   // ex: acme -> acme.conges-saas.com
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#2563EB";
    public string SecondaryColor { get; set; } = "#1E293B";
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<LeaveType> LeaveTypes { get; set; } = new List<LeaveType>();
    public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
    public TenantSettings? Settings { get; set; }
}
