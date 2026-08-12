using GestionConges.Domain.Entities;

namespace GestionConges.Application.Interfaces;

public interface IJwtToken
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}
