using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestionConges.Infrastructure.Security;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration) => _configuration = configuration;

    public (string token, DateTime expiresAt) GenerateToken(User user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // TenantId absent pour le SuperAdmin (plateforme), présent pour les autres rôles.
        if (user.TenantId is not null)
            claims.Add(new Claim("tenantId", user.TenantId.Value.ToString()));

        var expiresAt = DateTime.UtcNow.AddHours(double.Parse(jwtSection["ExpiryHours"] ?? "8"));

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSection["Secret"]!);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = !string.IsNullOrEmpty(jwtSection["Issuer"]),
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = !string.IsNullOrEmpty(jwtSection["Audience"]),
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }

    public bool ValidateToken(string token)
    {
        var principal = GetPrincipalFromToken(token);
        return principal?.Identity?.IsAuthenticated ?? false;
    }

    public Guid? GetUserIdFromToken(string token)
    {
        var principal = GetPrincipalFromToken(token);
        var idClaim = principal?.FindFirst(ClaimTypes.NameIdentifier) ?? principal?.FindFirst(JwtRegisteredClaimNames.Sub);
        if (idClaim is null) return null;
        return Guid.TryParse(idClaim.Value, out var guid) ? guid : null;
    }
}
