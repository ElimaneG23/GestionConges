using GestionConges.Application.Common;
using GestionConges.Application.DTOs.LeaveType;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;

public class LeaveTypeService : ILeaveTypeService
{
    private readonly IAppDbContext _db;

    public LeaveTypeService(IAppDbContext db) => _db = db;

    public async Task<Result<LeaveTypeDto>> CreateAsync(Guid tenantId, CreateLeaveTypeDto dto)
    {
        if (await _db.LeaveTypes.AnyAsync(l => l.TenantId == tenantId && l.Name.ToLower() == dto.Name.ToLower()))
            return Result<LeaveTypeDto>.Fail("Un type de congé avec ce nom existe déjà.");

        var entity = new LeaveType
        {
            TenantId = tenantId,
            Name = dto.Name,
            Description = dto.Description,
            DefaultDaysPerYear = dto.DefaultDaysPerYear,
            RequiresJustification = dto.RequiresJustification,
            IsActive = true
        };
        await _db.LeaveTypes.AddAsync(entity);
        await _db.SaveChangesAsync();
        return Result<LeaveTypeDto>.Ok(MapDto(entity));
    }

    public async Task<Result<List<LeaveTypeDto>>> GetAllAsync(Guid tenantId)
    {
        var list = await _db.LeaveTypes.Where(l => l.TenantId == tenantId)
            .OrderBy(l => l.Name).ToListAsync();
        return Result<List<LeaveTypeDto>>.Ok(list.Select(MapDto).ToList());
    }

    public async Task<Result<LeaveTypeDto>> UpdateAsync(Guid id, UpdateLeaveTypeDto dto)
    {
        var entity = await _db.LeaveTypes.FirstOrDefaultAsync(l => l.Id == id);
        if (entity is null) return Result<LeaveTypeDto>.Fail("Type de congé introuvable.");

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.DefaultDaysPerYear = dto.DefaultDaysPerYear;
        entity.RequiresJustification = dto.RequiresJustification;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Result<LeaveTypeDto>.Ok(MapDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var entity = await _db.LeaveTypes.FirstOrDefaultAsync(l => l.Id == id);
        if (entity is null) return Result<bool>.Fail("Type de congé introuvable.");
        entity.IsActive = false; // suppression logique pour préserver l'historique
        await _db.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }

    private static LeaveTypeDto MapDto(LeaveType l) => new(
        l.Id, l.Name, l.Description, l.DefaultDaysPerYear, l.RequiresJustification, l.IsActive);
}
