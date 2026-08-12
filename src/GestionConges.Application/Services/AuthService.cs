using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Auth;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using GestionConges.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;


public class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IJwtToken _jwtToken;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IAppDbContext db,
        IJwtToken jwtToken,
        IPasswordHasher passwordHasher)
    {
        _db = db;
        _jwtToken = jwtToken;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        try
        {
            // 1. Trouver le tenant
            var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Subdomain == dto.Subdomain.ToLower());
            if (tenant == null)
                return Result<LoginResponseDto>.Fail("Tenant non trouvé");

            // 2. Trouver l'utilisateur
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.TenantId == tenant.Id);
            if (user == null)
                return Result<LoginResponseDto>.Fail("Email ou mot de passe incorrect");

            // 3. Vérifier le mot de passe
            if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
                return Result<LoginResponseDto>.Fail("Email ou mot de passe incorrect");

            // 4. Vérifier le statut
            if (!user.IsActive || !tenant.IsActive)
                return Result<LoginResponseDto>.Fail("Compte inactif");

            // 5. Générer le token
            var (token, expiresAt) = _jwtToken.GenerateToken(user);

            // 6. Créer la réponse
            var response = new LoginResponseDto
            {
                Token = token,
                TokenExpiry = expiresAt,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    TenantId = user.TenantId ?? Guid.Empty,
                    RemainingLeaveDays = 0
                },
                Tenant = new TenantInfoDto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Subdomain = tenant.Subdomain,
                    LogoUrl = tenant.LogoUrl,
                    PrimaryColor = tenant.PrimaryColor,
                    SecondaryColor = tenant.SecondaryColor
                }
            };

            return Result<LoginResponseDto>.Ok(response);
        }
        catch (Exception ex)
        {
            return Result<LoginResponseDto>.Fail($"Erreur lors de la connexion: {ex.Message}");
        }
    }

    public async Task<Result<TenantRegistrationResponseDto>> RegisterTenantAsync(RegisterTenantRequestDto dto)
    {
        try
        {
            // Vérifier si le sous-domaine existe déjà
            if (await _db.Tenants.AnyAsync(t => t.Subdomain == dto.Subdomain.ToLower()))
                return Result<TenantRegistrationResponseDto>.Fail("Ce sous-domaine est déjà utilisé");

            // Créer le tenant
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = dto.CompanyName,
                Subdomain = dto.Subdomain.ToLower(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _db.Tenants.AddAsync(tenant);

            // Créer l'admin
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.AdminEmail,
                FirstName = dto.AdminFirstName,
                LastName = dto.AdminLastName,
                PasswordHash = _passwordHasher.Hash(dto.AdminPassword),
                Role = UserRole.Admin,
                TenantId = tenant.Id,
                HireDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _db.Users.AddAsync(admin);
            await _db.SaveChangesAsync();

            // Créer les types de congés par défaut
            // ... (créer des LeaveType par défaut)

            var response = new TenantRegistrationResponseDto
            {
                TenantId = tenant.Id,
                Subdomain = tenant.Subdomain,
                CompanyName = tenant.Name,
                AdminId = admin.Id,
                AdminEmail = admin.Email,
                Message = "Tenant créé avec succès"
            };

            return Result<TenantRegistrationResponseDto>.Ok(response);
        }
        catch (Exception ex)
        {
            return Result<TenantRegistrationResponseDto>.Fail($"Erreur: {ex.Message}");
        }
    }

    // ... Implémenter les autres méthodes
}