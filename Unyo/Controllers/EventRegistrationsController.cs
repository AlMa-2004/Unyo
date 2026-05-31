using Microsoft.AspNetCore.Mvc;
using Unyo.Models;
using Unyo.Services;

namespace Unyo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventRegistrationsController : ControllerBase
{
    private readonly IEventRegistrationService _registrationService;

    public EventRegistrationsController(IEventRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    // GET: api/eventregistrations/history/user123
    [HttpGet("history/{userId}")]
    public async Task<ActionResult<IEnumerable<EventRegistration>>> GetHistory(string userId, CancellationToken cancellationToken)
    {
        var history = await _registrationService.GetUserRegistrationHistoryAsync(userId, cancellationToken);
        return Ok(history);
    }

    // POST: api/eventregistrations
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] EventRegistration registration, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _registrationService.RegisterUserToEventAsync(registration, cancellationToken);
        if (!success)
            return BadRequest("The ticket does not exist or the registration could not be completed.");

        return Ok(new { message = "The registration was successful.", data = registration });
    }
}