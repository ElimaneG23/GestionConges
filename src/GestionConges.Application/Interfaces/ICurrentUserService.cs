using GestionConges.Domain.Enums;

namespace GestionConges.Application.Interfaces
{
    /// <summary>
    /// Fournit le contexte de l'utilisateur courant (extrait du JWT) : identité, entreprise (tenant), rôle.
    /// Implémenté dans Infrastructure via IHttpContextAccessor.
    /// C'est la pierre angulaire du multi-tenant : chaque requête est automatiquement
    /// cantonnée aux données de son entreprise (sauf le SuperAdmin).
    /// </summary>
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? CompanyId { get; }
        UserRole? Role { get; }
        Guid? EmployeeId { get; }
        bool IsAuthenticated { get; }
    }
}
