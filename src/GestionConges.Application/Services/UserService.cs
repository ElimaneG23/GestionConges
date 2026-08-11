using GestionConges.Application.Common;
using GestionConges.Application.DTOs.User;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using GestionConges.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;

/// <summary>
/// Gestion des employés/managers d'une entreprise, réservée à l'Admin (Sprint 2).
/// </summary>
public class UserService : IUserService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IAppDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> CreateEmployeeAsync(Guid tenantId, CreateEmployeeDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
            return Result<UserDto>.Fail("Cet email est déjà utilisé.");

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role) ||
            (role != UserRole.Manager && role != UserRole.Employee))
            return Result<UserDto>.Fail("Rôle invalide. Utilisez 'Manager' ou 'Employee'.");

        if (dto.ManagerId is not null)
        {
            var managerExists = await _db.Users.AnyAsync(u =>
                u.Id == dto.ManagerId && u.TenantId == tenantId &&
                (u.Role == UserRole.Manager || u.Role == UserRole.Admin));
            if (!managerExists)
                return Result<UserDto>.Fail("Manager introuvable pour cette entreprise.");
        }

        var user = new User
        {
            TenantId = tenantId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            Role = role,
            ManagerId = dto.ManagerId,
            JobTitle = dto.JobTitle,
            HireDate = dto.HireDate,
            IsActive = true
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return Result<UserDto>.Ok(await ToDtoAsync(user));
    }

    public async Task<Result<List<UserDto>>> GetEmployeesAsync(Guid tenantId, string? search, string? role)
    {
        var query = _db.Users.Include(u => u.Manager)
            .Where(u => u.TenantId == tenantId && u.Role != UserRole.Admin);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(u => u.FirstName.ToLower().Contains(s)
                || u.LastName.ToLower().Contains(s) || u.Email.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, true, out var r))
            query = query.Where(u => u.Role == r);

        var users = await query.OrderBy(u => u.LastName).ToListAsync();
        return Result<List<UserDto>>.Ok(users.Select(user => MapDto(user)).ToList());
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _db.Users.Include(u => u.Manager).FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return Result<UserDto>.Fail("Utilisateur introuvable.");
        return Result<UserDto>.Ok(MapDto(user));
    }

    public async Task<Result<UserDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto dto)
    {
        var user = await _db.Users.Include(u => u.Manager).FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return Result<UserDto>.Fail("Utilisateur introuvable.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.JobTitle = dto.JobTitle;
        user.ManagerId = dto.ManagerId;
        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Result<UserDto>.Ok(MapDto(user));
    }

    public async Task<Result<bool>> DeactivateAsync(Guid id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return Result<bool>.Fail("Utilisateur introuvable.");
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }

    private async Task<UserDto> ToDtoAsync(User user)
    {
        User? manager = user.ManagerId is null ? null
            : await _db.Users.FirstOrDefaultAsync(u => u.Id == user.ManagerId);
        return MapDto(user, manager);
    }

    private static UserDto MapDto(User u, User? manager = null) => new(
        u.Id, u.FirstName, u.LastName, u.Email, u.Role.ToString(), u.IsActive,
        u.ManagerId, (manager ?? u.Manager)?.FullName, u.JobTitle, u.HireDate, u.CreatedAt);
}
