using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unyo.Models;
using Unyo.Services;

namespace Unyo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    // HELPER PRIVAT PENTRU EVENIMENTE
    private bool IsEventOwnerOrAdmin(Event @event)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return @event.UserId == currentUserId || User.IsInRole("Admin");
    }

    // GET: api/events
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents(CancellationToken cancellationToken)
    {
        var events = await _eventService.GetAllEventsAsync(cancellationToken);
        return Ok(events);
    }

    // GET: api/events/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Event>> GetEvent(int id, CancellationToken cancellationToken)
    {
        var @event = await _eventService.GetEventByIdAsync(id, cancellationToken);
        if (@event == null)
            return NotFound();

        return Ok(@event);
    }

    // POST: api/events
    [HttpPost]
    [Authorize(Roles = "Admin,Vendor")]
    public async Task<ActionResult<Event>> CreateEvent(Event @event, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin"))
        {
            @event.UserId = currentUserId ?? string.Empty;
        }

        var createdEvent = await _eventService.CreateEventAsync(@event, cancellationToken);

        return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
    }

    // DELETE: api/events/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Vendor")]
    public async Task<IActionResult> DeleteEvent(int id, CancellationToken cancellationToken)
    {
        var @event = await _eventService.GetEventByIdAsync(id, cancellationToken);
        if (@event == null)
            return NotFound();

        if (!IsEventOwnerOrAdmin(@event))
        {
            return Forbid();
        }

        var result = await _eventService.DeleteEventAsync(id, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}