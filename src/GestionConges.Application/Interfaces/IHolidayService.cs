using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Holiday;

namespace GestionConges.Application.Interfaces;

public interface IHolidayService
{
    Task<Result<HolidayDto>> CreateAsync(Guid tenantId, CreateHolidayDto dto);
    Task<Result<List<HolidayDto>>> GetAllAsync(Guid tenantId);
    Task<Result<bool>> DeleteAsync(Guid id);
}
