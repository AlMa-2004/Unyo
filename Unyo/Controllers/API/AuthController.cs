using Microsoft.AspNetCore.Mvc;
using Unyo.Dtos;
//using Unyo.Services;

namespace Unyo.Controllers.API;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // private readonly IAuthService _authService; 
    // public AuthController(IAuthService authService) { _authService = authService; }

    // POST: api/auth/register
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // var result = await _authService.RegisterAsync(dto);
        // if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok(new { message = "User registered successfully!" });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // var token = await _authService.LoginAsync(dto);
        // if (token == null) return Unauthorized("Invalid email or password.");

        string mockToken = "fake-jwt-token-for-testing";
        return Ok(new { token = mockToken });
    }
}