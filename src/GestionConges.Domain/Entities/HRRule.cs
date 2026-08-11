using GestionConges.Domain.Common;
using GestionConges.Domain.Enums;

namespace GestionConges.Domain.Entities
{
    /// <summary>
    /// Règle RH paramétrable par entreprise (US9.1), modèle clé/valeur pour rester
    /// extensible sans changer le schéma (ex: MaxConsecutiveDays = "20").
    /// </summary>
    public class HRRule : TenantEntity
    {
        public HRRuleKey Key { get; set; }
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
