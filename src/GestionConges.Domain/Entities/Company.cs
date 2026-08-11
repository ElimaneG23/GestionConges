using GestionConges.Domain.Common;

namespace GestionConges.Domain.Entities
{
    /// <summary>
    /// Représente une entreprise cliente de la plateforme SaaS (un "tenant").
    /// Créée par le Super Admin, personnalisée ensuite par l'Admin de l'entreprise.
    /// </summary>
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>Sous-domaine ou identifiant unique utilisé pour résoudre le tenant (ex: acme.gestion-conges.com)</summary>
        public string Slug { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }
        public string PrimaryColor { get; set; } = "#2563EB";
        public string SecondaryColor { get; set; } = "#1E293B";

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<LeaveType> LeaveTypes { get; set; } = new List<LeaveType>();
        public virtual ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
        public virtual ICollection<HRRule> HRRules { get; set; } = new List<HRRule>();
    }
}
