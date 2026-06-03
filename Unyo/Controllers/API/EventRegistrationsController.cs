using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unyo.Models;
using Unyo.Services;
using Unyo.Dtos;
using Unyo.Mappings;

namespace Unyo.Controllers.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventRegistrationsController : ControllerBase
{
    private readonly IEventRegistrationService _registrationService;

    public EventRegistrationsController(IEventRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }
    private bool IsRegistrationOwnerOrAdmin(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId == currentUserId || User.IsInRole("Admin");
    }

    // GET: api/eventregistrations/history/user123
    [HttpGet("history/{userId}")]
    [ProducesResponseType(typeof(List<RegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> GetHistory(string userId, CancellationToken cancellationToken)
    {
        if (!IsRegistrationOwnerOrAdmin(userId))
        {
            return Forbid();
        }

        var history = await _registrationService.GetUserRegistrationHistoryAsync(userId, cancellationToken);
        var dtos = history.Select(r => r.MapToDto());

        return Ok(dtos);
    }

    // POST: api/eventregistrations
    [HttpPost]
    [Authorize(Roles = "User,Admin,Vendor")]
    [ProducesResponseType(typeof(RegistrationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RegistrationDto>> Register([FromBody] CreateRegistrationDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Map DTO to Entity
        var registration = dto.MapToEntity();

        // Extract the authenticated user's ID from the JWT token
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        registration.UserId = currentUserId ?? string.Empty;

        // Double-check using your helper function (Defense in depth)
        if (!IsRegistrationOwnerOrAdmin(registration.UserId))
        {
            return Forbid();
        }

        // Save to database
        var success = await _registrationService.RegisterUserToEventAsync(registration, cancellationToken);
        if (!success)
            return BadRequest("The ticket does not exist or the registration could not be completed.");

        return Ok(registration.MapToDto());
    }
}