using GestionConges.Domain.Common;
using GestionConges.Domain.Enums;

namespace GestionConges.Domain.Entities;

/// <summary>
/// Utilisateur de la plateforme. TenantId est nul pour le SuperAdmin
/// (qui n'appartient à aucune entreprise, il gère la plateforme elle-même).
/// </summary>
public class User : BaseEntity
{
    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    // Un employé est rattaché à un manager (auto-référence)
    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }
    public ICollection<User> Subordinates { get; set; } = new List<User>();

    public string? JobTitle { get; set; }
    public DateTime? HireDate { get; set; }

    // Navigation
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public string FullName => $"{FirstName} {LastName}";

    public decimal RemainingLeaveDays { get; set; } = 30m;
}
