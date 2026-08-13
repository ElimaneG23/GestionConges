using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionConges.Application.Services;

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
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName ?? $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("userId", user.Id.ToString()),
            new("email", user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
}