using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using global::Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionConges.Application.Services;

public class JwtTokenService : IJwtToken
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiresAt) GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var expiryHours = double.Parse(jwtSettings["ExpiryHours"] ?? "8");
        var expiresAt = DateTime.UtcNow.AddHours(expiryHours);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.FullName) ? user.FirstName : user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.TenantId is not null)
        {
            claims.Add(new("tenantId", user.TenantId.Value.ToString()));
        }

        var secret = jwtSettings["Secret"]
            ?? throw new InvalidOperationException(
                "La clé JWT est manquante dans la configuration."
            );
        var keyValue = jwtSettings["Key"]
            ?? throw new InvalidOperationException(
                "La clé JWT est manquante dans la configuration."
            );

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(keyValue)
        );

        // Credentials de signature
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        // Création du token
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        // Transformation du JWT en chaîne de caractères
        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return (tokenString, expiresAt);
    }
}