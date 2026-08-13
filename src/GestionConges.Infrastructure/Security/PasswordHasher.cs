using BCrypt.Net;
using GestionConges.Application.Interfaces;

namespace GestionConges.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            // Utiliser BCrypt avec la version 2b (plus compatible)
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool Verify(string passwordHash, string password)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Si le salt est invalide, recréer le hash
                return false;
            }
        }
    }
}