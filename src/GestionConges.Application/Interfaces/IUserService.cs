using GestionConges.Application.Common;
using GestionConges.Application.DTOs.User;

namespace GestionConges.Application.Interfaces;

public interface IUserService
{
    Task<Result<UserDto>> CreateEmployeeAsync(Guid tenantId, CreateEmployeeDto dto);
    Task<Result<List<UserDto>>> GetEmployeesAsync(Guid tenantId, string? search, string? role);
    Task<Result<UserDto>> GetByIdAsync(Guid id);
    Task<Result<UserDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto dto);
    Task<Result<bool>> DeactivateAsync(Guid id);
}
