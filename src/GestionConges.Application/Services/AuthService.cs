using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Auth;
using GestionConges.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(IAppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _db.Users
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponseDto>.Fail("Email ou mot de passe incorrect.");

        if (!user.IsActive)
            return Result<LoginResponseDto>.Fail("Ce compte est désactivé.");

        if (user.Tenant is not null && !user.Tenant.IsActive)
            return Result<LoginResponseDto>.Fail("Cette entreprise est désactivée. Contactez le support.");

        var (token, expiresAt) = _jwtService.GenerateToken(user);

        var response = new LoginResponseDto(
            token,
            expiresAt,
            user.Id,
            user.FullName,
            user.Role.ToString(),
            user.TenantId,
            user.Tenant?.Name
        );

        return Result<LoginResponseDto>.Ok(response);
    }
}
