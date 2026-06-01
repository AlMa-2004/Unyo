using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unyo.Models;
using Unyo.Services;

namespace Unyo.Controllers;

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

    private bool IsRegistrationOwnerOrAdmin(EventRegistration registration)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return registration.UserId == currentUserId || User.IsInRole("Admin");
    }

    // GET: api/eventregistrations/history/user123
    [HttpGet("history/{userId}")]
    public async Task<ActionResult<IEnumerable<EventRegistration>>> GetHistory(string userId, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var history = await _registrationService.GetUserRegistrationHistoryAsync(userId, cancellationToken);
        return Ok(history);
    }

    // POST: api/eventregistrations
    [HttpPost]
    [Authorize(Roles = "User,Admin,Vendor")]
    public async Task<IActionResult> Register([FromBody] EventRegistration registration, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!IsRegistrationOwnerOrAdmin(registration))
        {
            return Forbid(); // Forbidden 403 if the user is not the owner of the registration and not an admin
        }

        var success = await _registrationService.RegisterUserToEventAsync(registration, cancellationToken);
        if (!success)
            return BadRequest("The ticket does not exist or the registration could not be completed.");

        return Ok(new { message = "The registration was successful.", data = registration });
    }
}