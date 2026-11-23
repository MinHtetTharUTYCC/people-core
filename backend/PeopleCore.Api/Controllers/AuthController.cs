using Microsoft.AspNetCore.Mvc;
using PeopleCore.Application.Identity.DTOs;
using PeopleCore.Application.Identity.Services;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        if (result.Success)
            return Created(string.Empty, result);
        return BadRequest(result);
    }
}
