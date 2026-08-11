using GestionConges.API.Common;
using GestionConges.Application.DTOs.Auth;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionConges.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>US1.1 - Connexion utilisateur avec JWT (tous rôles).</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result.ToActionResult(this);
    }
}
