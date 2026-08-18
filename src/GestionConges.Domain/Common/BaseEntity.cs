using GestionConges.Domain.Entities;
using GestionConges.Domain.Enums;

namespace GestionConges.Domain.Common;

/// <summary>
/// Racine commune : identifiant Guid + traçabilité de création/mise à jour.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
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

    // ✅ UNE SEULE FOIS - Dernière connexion
    public DateTime? LastLoginAt { get; set; }
}
