using GestionConges.Application.Common;
using GestionConges.Application.DTOs.LeaveType;

namespace GestionConges.Application.Interfaces;

public interface ILeaveTypeService
{
    Task<Result<LeaveTypeDto>> CreateAsync(Guid tenantId, CreateLeaveTypeDto dto);
    Task<Result<List<LeaveTypeDto>>> GetAllAsync(Guid tenantId);
    Task<Result<LeaveTypeDto>> UpdateAsync(Guid id, UpdateLeaveTypeDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
}
