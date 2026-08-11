using System.Linq.Expressions;

namespace GestionConges.Application.Interfaces;

/// <summary>
/// Repository générique (pattern Repository) au-dessus d'EF Core, utilisé par
/// tous les repositories spécifiques pour éviter la duplication de code CRUD.
/// </summary>
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync();
}
