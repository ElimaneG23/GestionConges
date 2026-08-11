namespace GestionConges.Domain.Enums
{
    /// <summary>
    /// Clés de règles RH paramétrables par entreprise (Sprint 9 - Administration).
    /// Stockées en table clé/valeur pour rester extensibles sans migration de schéma.
    /// </summary>
    public enum HRRuleKey
    {
        MaxConsecutiveDays,
        MinNoticeDaysBeforeRequest,
        AllowNegativeBalance,
        CarryOverDaysLimit,
        FiscalYearStartMonth
    }
}
