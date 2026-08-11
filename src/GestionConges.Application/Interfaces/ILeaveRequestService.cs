using GestionConges.Application.Common;
using GestionConges.Application.DTOs.LeaveRequest;

namespace GestionConges.Application.Interfaces;

public interface ILeaveRequestService
{
    Task<Result<LeaveRequestDto>> CreateAsync(Guid userId, Guid tenantId, CreateLeaveRequestDto dto);
    Task<Result<List<LeaveRequestDto>>> GetMyRequestsAsync(Guid userId);
    Task<Result<bool>> CancelAsync(Guid requestId, Guid userId);

    // Validation manager (Sprint 5)
    Task<Result<List<LeaveRequestDto>>> GetPendingForManagerAsync(Guid managerId);
    Task<Result<LeaveRequestDto>> ProcessAsync(Guid requestId, Guid managerId, ProcessLeaveRequestDto dto);
}
