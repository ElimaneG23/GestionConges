using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Dashboard;

namespace GestionConges.Application.Interfaces;

public interface IDashboardService
{
    Task<Result<EmployeeDashboardDto>> GetEmployeeDashboardAsync(Guid userId);
    Task<Result<ManagerDashboardDto>> GetManagerDashboardAsync(Guid managerId);
    Task<Result<AdminDashboardDto>> GetAdminDashboardAsync(Guid tenantId);
    Task<Result<SuperAdminDashboardDto>> GetSuperAdminDashboardAsync();
}
