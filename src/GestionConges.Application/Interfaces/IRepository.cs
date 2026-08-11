using System.Linq.Expressions;
using GestionConges.Domain.Common;

namespace GestionConges.Application.Interfaces
{
    /// <summary>
    /// Repository générique (pattern Repository) au-dessus d'EF Core.
    /// Le filtre multi-tenant (CompanyId) est appliqué automatiquement par
    /// l'implémentation Infrastructure pour les entités de type TenantEntity.
    /// </summary>
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> Query(bool track = false);
        Task<T?> GetByIdAsync(Guid id);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
    }
}
