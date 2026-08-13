using GestionConges.Domain.Entities;

namespace GestionConges.Domain.Common;

/// <summary>
/// Racine commune : identifiant Guid + traçabilité de création/mise à jour.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}

/// <summary>
/// Racine pour toute entité qui appartient à une entreprise (tenant) du SaaS.
/// Permet l'isolation multi-tenant (filtrage automatique par TenantId).
/// </summary>
public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}
