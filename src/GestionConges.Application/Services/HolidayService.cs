using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Holiday;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;

public class HolidayService : IHolidayService
{
    private readonly IAppDbContext _db;
    public HolidayService(IAppDbContext db) => _db = db;

    public async Task<Result<HolidayDto>> CreateAsync(Guid tenantId, CreateHolidayDto dto)
    {
        var holiday = new Holiday
        {
            TenantId = tenantId,
            Name = dto.Name,
            Date = dto.Date,
            IsRecurringYearly = dto.IsRecurringYearly
        };
        await _db.Holidays.AddAsync(holiday);
        await _db.SaveChangesAsync();
        return Result<HolidayDto>.Ok(new HolidayDto(holiday.Id, holiday.Name, holiday.Date, holiday.IsRecurringYearly));
    }

    public async Task<Result<List<HolidayDto>>> GetAllAsync(Guid tenantId)
    {
        var list = await _db.Holidays.Where(h => h.TenantId == tenantId).OrderBy(h => h.Date).ToListAsync();
        return Result<List<HolidayDto>>.Ok(list.Select(h =>
            new HolidayDto(h.Id, h.Name, h.Date, h.IsRecurringYearly)).ToList());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var holiday = await _db.Holidays.FirstOrDefaultAsync(h => h.Id == id);
        if (holiday is null) return Result<bool>.Fail("Jour férié introuvable.");
        _db.Holidays.Remove(holiday);
        await _db.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }
}
