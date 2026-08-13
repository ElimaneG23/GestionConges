using GestionConges.API.Common;
using GestionConges.Application.DTOs.Auth;
using GestionConges.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GestionConges.API.Controllers;

[ApiController]
[Route("api/[controller]")]


public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(dto);
        return result.ToActionResult(this);
    }

    [HttpPost("register-tenant")]
    public async Task<IActionResult> RegisterTenant([FromBody] RegisterTenantRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterTenantAsync(dto);

        if (result.Success)
            return Created(string.Empty, result.Data);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Rafraîchir le token JWT
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RefreshTokenAsync(dto);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Déconnexion utilisateur
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _authService.LogoutAsync(Guid.Parse(userId));
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Vérifier si le token est valide
    /// </summary>
    //[HttpGet("validate-token")]
    //public async Task<IActionResult> ValidateToken()
    //{
    //    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    //    if (string.IsNullOrEmpty(token))
    //        return Unauthorized();

    //    var result = await _authService.ValidateTokenAsync(token);
    //    return result.ToActionResult(this);
    //}

}