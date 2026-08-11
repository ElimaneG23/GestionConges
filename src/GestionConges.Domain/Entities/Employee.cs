using GestionConges.Domain.Common;

namespace GestionConges.Domain.Entities
{
    /// <summary>
    /// Fiche employé (US2.1, US2.2, US2.3). Rattachée à un User pour l'authentification
    /// et, optionnellement, à un Manager (auto-référence) pour le circuit de validation.
    /// </summary>
    public class Employee : TenantEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public string MatriculeCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }

        /// <summary>Manager hiérarchique direct (peut être null si non affecté).</summary>
        public Guid? ManagerId { get; set; }
        public virtual Employee? Manager { get; set; }
        public virtual ICollection<Employee> DirectReports { get; set; } = new List<Employee>();

        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    }
}
