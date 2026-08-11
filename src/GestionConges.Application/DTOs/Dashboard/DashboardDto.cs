namespace GestionConges.Application.DTOs.Dashboard;

public record EmployeeDashboardDto(
    decimal TotalDaysAllocated,
    decimal TotalDaysUsed,
    decimal TotalDaysRemaining,
    int PendingRequests,
    List<LeaveRequest.LeaveRequestDto> RecentRequests
);

public record ManagerDashboardDto(
    int PendingApprovals,
    int TeamSize,
    int ApprovedThisMonth,
    List<LeaveRequest.LeaveRequestDto> RequestsToProcess,
    List<LeaveRequest.LeaveRequestDto> UpcomingTeamLeaves
);

public record AdminDashboardDto(
    int TotalEmployees,
    int TotalManagers,
    int PendingRequestsCompanyWide,
    int ApprovedThisMonth,
    int RejectedThisMonth,
    Dictionary<string, int> RequestsByLeaveType
);

public record SuperAdminDashboardDto(
    int TotalTenants,
    int ActiveTenants,
    int TotalUsersAcrossPlatform,
    List<Tenant.TenantDto> RecentTenants
);
