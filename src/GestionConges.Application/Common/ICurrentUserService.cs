using GestionConges.Domain.Enums;

namespace GestionConges.Application.Common;

/// <summary>
/// Expose l'utilisateur courant (extrait du token JWT) aux services applicatifs,
/// sans dépendre directement de HttpContext (respect de la Clean Architecture).
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    UserRole? Role { get; }
    bool IsAuthenticated { get; }
}
