using GestionConges.Application.Interfaces;

namespace GestionConges.Infrastructure.Security;

/// <summary>
/// Hachage de mot de passe via BCrypt.Net-Next (sel intégré, résistant au brute-force).
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
