using System.Security.Claims;
using GestionConges.Application.Common;
using GestionConges.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace GestionConges.Infrastructure.CurrentUser;

/// <summary>
/// Lit l'utilisateur courant depuis les claims du JWT (posé par le middleware d'auth).
/// C'est le mécanisme central de l'isolation multi-tenant : chaque requête ne "voit"
/// que les données de son propre TenantId (sauf le SuperAdmin, qui n'en a pas).
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid? TenantId
    {
        get
        {
            var value = User?.FindFirst("tenantId")?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public UserRole? Role
    {
        get
        {
            var value = User?.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse<UserRole>(value, out var role) ? role : null;
        }
    }
}
