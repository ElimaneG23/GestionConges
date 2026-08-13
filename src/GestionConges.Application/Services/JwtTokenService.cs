using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionConges.Application.Services;
 
#pragma warning disable CS8604
public class JwtTokenService : IJwtToken
{
    private readonly IConfiguration _configuration;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public (string token, DateTime expiresAt) GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        // Récupérer la clé (correction : utiliser "Key" pas "Secret")
        var keyValue = jwtSettings["Key"]
            ?? throw new InvalidOperationException("La clé JWT est manquante dans la configuration.");

        // Durée d'expiration
        var expiryHours = double.Parse(jwtSettings["ExpiryHours"] ?? "8");
        var expiresAt = DateTime.UtcNow.AddHours(expiryHours);

        // Claims de l'utilisateur
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FullName ?? (user.FirstName + " " + user.LastName) ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        // Ajouter le tenantId si présent
        if (user.TenantId != Guid.Empty)
        {
            claims.Add(new("tenantId", user.TenantId.ToString()));
        }

        // Ajouter les jours de congés restants
        claims.Add(new("remainingLeaveDays", user.RemainingLeaveDays.ToString()));

        // Création de la clé de signature
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Création du token
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "GestionConges.API",
            audience: jwtSettings["Audience"] ?? "GestionConges.Client",
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        // Transformation en string
        var tokenString = _tokenHandler.WriteToken(token);

        return (tokenString, expiresAt);
    }
    public string GenerateRefreshToken()
    {
        // Générer un refresh token aléatoire
        return $"{Guid.NewGuid():N}{Guid.NewGuid():N}";
    }
    public bool ValidateToken(string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyValue = jwtSettings["Key"]
                ?? throw new InvalidOperationException("La clé JWT est manquante.");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"] ?? "GestionConges.API",
                ValidAudience = jwtSettings["Audience"] ?? "GestionConges.Client",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue)),
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, tokenValidationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
#pragma warning restore CS8604