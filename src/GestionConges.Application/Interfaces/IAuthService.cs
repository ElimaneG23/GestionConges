using GestionConges.Application.Common;
using GestionConges.Application.DTOs.Auth;

namespace GestionConges.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
        Task<Result<TenantRegistrationResponseDto>> RegisterTenantAsync(RegisterTenantRequestDto dto);
    }
}