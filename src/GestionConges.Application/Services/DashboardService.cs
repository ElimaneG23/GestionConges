using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Dashboard;
using GestionConges.Application.DTOs.Tenant;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;

/// <summary>Sprint 6 : agrégations par rôle (employé / manager / admin / super admin).</summary>
public class DashboardService : IDashboardService
{
    private readonly IAppDbContext _db;
    public DashboardService(IAppDbContext db) => _db = db;

    public async Task<Result<EmployeeDashboardDto>> GetEmployeeDashboardAsync(Guid userId)
    {
        var year = DateTime.UtcNow.Year;
        var balances = await _db.LeaveBalances.Where(b => b.UserId == userId && b.Year == year).ToListAsync();

        var requests = await _db.LeaveRequests
            .Include(r => r.User).Include(r => r.LeaveType).Include(r => r.ProcessedByUser)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt).Take(5).ToListAsync();

        var pending = await _db.LeaveRequests.CountAsync(r => r.UserId == userId && r.Status == LeaveRequestStatus.Pending);

        var dto = new EmployeeDashboardDto(
            balances.Sum(b => b.TotalDays),
            balances.Sum(b => b.UsedDays),
            balances.Sum(b => b.RemainingDays),
            pending,
            requests.Select(r => new DTOs.LeaveRequest.LeaveRequestDto(
                r.Id, r.UserId, r.User!.FullName, r.LeaveTypeId, r.LeaveType!.Name,
                r.StartDate, r.EndDate, r.NumberOfDays, r.Reason, r.Status.ToString(),
                r.ManagerComment, r.ProcessedByUser?.FullName, r.ProcessedAt, r.CreatedAt)).ToList()
        );
        return Result<EmployeeDashboardDto>.Ok(dto);
    }

    public async Task<Result<ManagerDashboardDto>> GetManagerDashboardAsync(Guid managerId)
    {
        var pendingQuery = _db.LeaveRequests.Include(r => r.User).Include(r => r.LeaveType).Include(r => r.ProcessedByUser)
            .Where(r => r.Status == LeaveRequestStatus.Pending && r.User!.ManagerId == managerId);

        var pending = await pendingQuery.OrderBy(r => r.StartDate).ToListAsync();
        var teamSize = await _db.Users.CountAsync(u => u.ManagerId == managerId && u.IsActive);

        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var approvedThisMonth = await _db.LeaveRequests.CountAsync(r =>
            r.User!.ManagerId == managerId && r.Status == LeaveRequestStatus.Approved && r.ProcessedAt >= monthStart);

        var upcoming = await _db.LeaveRequests.Include(r => r.User).Include(r => r.LeaveType).Include(r => r.ProcessedByUser)
            .Where(r => r.User!.ManagerId == managerId && r.Status == LeaveRequestStatus.Approved && r.StartDate >= DateTime.UtcNow)
            .OrderBy(r => r.StartDate).Take(10).ToListAsync();

        static DTOs.LeaveRequest.LeaveRequestDto Map(Domain.Entities.LeaveRequest r) => new(
            r.Id, r.UserId, r.User!.FullName, r.LeaveTypeId, r.LeaveType!.Name,
            r.StartDate, r.EndDate, r.NumberOfDays, r.Reason, r.Status.ToString(),
            r.ManagerComment, r.ProcessedByUser?.FullName, r.ProcessedAt, r.CreatedAt);

        var dto = new ManagerDashboardDto(
            pending.Count, teamSize, approvedThisMonth,
            pending.Select(Map).ToList(), upcoming.Select(Map).ToList());

        return Result<ManagerDashboardDto>.Ok(dto);
    }

    public async Task<Result<AdminDashboardDto>> GetAdminDashboardAsync(Guid tenantId)
    {
        var totalEmployees = await _db.Users.CountAsync(u => u.TenantId == tenantId && u.Role == UserRole.Employee);
        var totalManagers = await _db.Users.CountAsync(u => u.TenantId == tenantId && u.Role == UserRole.Manager);
        var pending = await _db.LeaveRequests.CountAsync(r => r.TenantId == tenantId && r.Status == LeaveRequestStatus.Pending);

        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var approved = await _db.LeaveRequests.CountAsync(r =>
            r.TenantId == tenantId && r.Status == LeaveRequestStatus.Approved && r.ProcessedAt >= monthStart);
        var rejected = await _db.LeaveRequests.CountAsync(r =>
            r.TenantId == tenantId && r.Status == LeaveRequestStatus.Rejected && r.ProcessedAt >= monthStart);

        var byType = await _db.LeaveRequests.Include(r => r.LeaveType)
            .Where(r => r.TenantId == tenantId)
            .GroupBy(r => r.LeaveType!.Name)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .ToListAsync();

        var dto = new AdminDashboardDto(totalEmployees, totalManagers, pending, approved, rejected,
            byType.ToDictionary(x => x.Name, x => x.Count));

        return Result<AdminDashboardDto>.Ok(dto);
    }

    public async Task<Result<SuperAdminDashboardDto>> GetSuperAdminDashboardAsync()
    {
        var totalTenants = await _db.Tenants.CountAsync();
        var activeTenants = await _db.Tenants.CountAsync(t => t.IsActive);
        var totalUsers = await _db.Users.CountAsync(u => u.Role != UserRole.SuperAdmin);

        var recent = await _db.Tenants.OrderByDescending(t => t.CreatedAt).Take(5).ToListAsync();
        var recentDtos = new List<TenantDto>();
        foreach (var t in recent)
        {
            var count = await _db.Users.CountAsync(u => u.TenantId == t.Id);
            recentDtos.Add(new TenantDto(t.Id, t.Name, t.Subdomain, t.LogoUrl, t.PrimaryColor,
                t.SecondaryColor, t.IsActive, t.CreatedAt, count));
        }

        return Result<SuperAdminDashboardDto>.Ok(
            new SuperAdminDashboardDto(totalTenants, activeTenants, totalUsers, recentDtos));
    }
}
