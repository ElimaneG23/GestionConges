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

}