using GestionConges.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Interfaces;

/// <summary>
/// Abstraction du DbContext exposée à la couche Application pour respecter la
/// Clean Architecture (Application ne référence pas EF Core / Infrastructure directement).
/// </summary>
public interface IAppDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<TenantSettings> TenantSettings { get; }
    DbSet<User> Users { get; }
    DbSet<LeaveType> LeaveTypes { get; }
    DbSet<LeaveBalance> LeaveBalances { get; }
    DbSet<LeaveRequest> LeaveRequests { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Holiday> Holidays { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
