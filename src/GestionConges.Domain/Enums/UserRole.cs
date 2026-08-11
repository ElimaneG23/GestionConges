namespace GestionConges.Domain.Enums;

/// <summary>
/// Rôles applicatifs. La hiérarchie SaaS est :
/// SuperAdmin (édite la plateforme) > Admin (entreprise / tenant) > Manager > Employee
/// </summary>
public enum UserRole
{
    SuperAdmin = 0,
    Admin = 1,
    Manager = 2,
    Employee = 3
}
