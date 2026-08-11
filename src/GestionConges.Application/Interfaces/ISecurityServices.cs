using GestionConges.Domain.Entities;

namespace GestionConges.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string token, DateTime expiresAt) GenerateToken(User user);
    }
}
