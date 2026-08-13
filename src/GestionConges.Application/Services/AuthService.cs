using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Auth;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using GestionConges.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtToken _jwtToken;

    public AuthService(
        IAppDbContext context,
        IPasswordHasher passwordHasher,
        IJwtToken jwtToken)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtToken = jwtToken;
    }

    // ==========================================
    // LOGIN
    // ==========================================

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        try
        {
            // 1. Trouver le tenant
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == dto.Subdomain.ToLower());

            if (tenant == null)
                return Result<LoginResponseDto>.Fail("Tenant non trouvé");

            if (!tenant.IsActive)
                return Result<LoginResponseDto>.Fail("Ce tenant n'est pas actif");

            // 2. Trouver l'utilisateur
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.TenantId == tenant.Id);

            if (user == null)
                return Result<LoginResponseDto>.Fail("Email ou mot de passe incorrect");

            // 3. Vérifier le statut
            if (!user.IsActive)
                return Result<LoginResponseDto>.Fail("Compte inactif");

            // 4. Vérifier le mot de passe
            if (!_passwordHasher.Verify(user.PasswordHash, dto.Password))
                return Result<LoginResponseDto>.Fail("Email ou mot de passe incorrect");

            // 5. Générer le token JWT
            var (token, expiresAt) = _jwtToken.GenerateToken(user);

            // 6. Créer la réponse
            var response = new LoginResponseDto
            {
                Token = token,
                RefreshToken = string.Empty,
                TokenExpiry = expiresAt,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    TenantId = user.TenantId ?? Guid.Empty,
                    RemainingLeaveDays = user.RemainingLeaveDays switch
                    {
                        decimal d => d,
                        //int i => i,
                        //long l => l,
                        //string s when decimal.TryParse(s, out var parsed) => parsed,
                        //_ => 0m
                    }
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

    // ==========================================
    // REGISTER TENANT
    // ==========================================

    public async Task<Result<TenantRegistrationResponseDto>> RegisterTenantAsync(RegisterTenantRequestDto dto)
    {
        try
        {
            // Vérifier si le sous-domaine existe déjà
            var existingTenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == dto.Subdomain.ToLower());

            if (existingTenant != null)
                return Result<TenantRegistrationResponseDto>.Fail("Ce sous-domaine est déjà utilisé");

            // Vérifier si l'email existe déjà
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.AdminEmail);

            if (existingUser != null)
                return Result<TenantRegistrationResponseDto>.Fail("Cet email est déjà utilisé");

            // Créer le tenant
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = dto.CompanyName,
                Subdomain = dto.Subdomain.ToLower(),
                PrimaryColor = "#2563EB",
                SecondaryColor = "#1E293B",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Tenants.AddAsync(tenant);

            // Créer l'admin
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.AdminEmail,
                FirstName = dto.AdminFirstName,
                LastName = dto.AdminLastName,
                PasswordHash = _passwordHasher.Hash(dto.AdminPassword),
                Role = UserRole.Admin,
                IsActive = true,
                TenantId = tenant.Id,
                HireDate = DateTime.UtcNow,
                RemainingLeaveDays = 30m,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(admin);

            // Créer les types de congés par défaut
            var defaultLeaveTypes = new[]
            {
                new LeaveType
                {
                    Id = Guid.NewGuid(),
                    Name = "Congés payés",
                    Description = "Congés annuels payés",
                    DefaultDaysPerYear = 25,
                    RequiresJustification = false,
                    IsActive = true,
                    TenantId = tenant.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new LeaveType
                {
                    Id = Guid.NewGuid(),
                    Name = "RTT",
                    Description = "Réduction du temps de travail",
                    DefaultDaysPerYear = 12,
                    RequiresJustification = false,
                    IsActive = true,
                    TenantId = tenant.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new LeaveType
                {
                    Id = Guid.NewGuid(),
                    Name = "Maladie",
                    Description = "Arrêt maladie",
                    DefaultDaysPerYear = 0,
                    RequiresJustification = false,
                    IsActive = true,
                    TenantId = tenant.Id,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.LeaveTypes.AddRangeAsync(defaultLeaveTypes);
            await _context.SaveChangesAsync();

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
            return Result<TenantRegistrationResponseDto>.Fail($"Erreur lors de la création du tenant: {ex.Message}");
        }
    }

    public Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto)
    {
        throw new NotImplementedException();
    }
}