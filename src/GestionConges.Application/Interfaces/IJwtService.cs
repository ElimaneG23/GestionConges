using GestionConges.Domain.Entities;

namespace GestionConges.Application.Interfaces;

public interface IJwtService
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}
