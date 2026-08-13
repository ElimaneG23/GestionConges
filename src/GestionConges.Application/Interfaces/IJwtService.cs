using GestionConges.Domain.Entities;
using System.Security.Claims;

namespace GestionConges.Application.Interfaces
{
    public interface IJwtService
    {
        /// <summary>
        /// Génère un token JWT pour l'utilisateur
        /// </summary>
        (string token, DateTime expiresAt) GenerateToken(User user);

        /// <summary>
        /// Génère un refresh token
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Valide un token JWT
        /// </summary>
        bool ValidateToken(string token);

        /// <summary>
        /// Récupère le ClaimsPrincipal depuis un token
        /// </summary>
        ClaimsPrincipal GetPrincipalFromToken(string token);

        /// <summary>
        /// Récupère l'ID utilisateur depuis un token
        /// </summary>
        Guid? GetUserIdFromToken(string token);
    }
}