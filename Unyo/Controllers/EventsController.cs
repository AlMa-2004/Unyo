using Microsoft.AspNetCore.Mvc;
using Unyo.Models;
using Unyo.Services;

namespace Unyo.Controllers;

[ApiController]
[Route("api/[controller]")] // Endpoint-ul va fi: api/events
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    // GET: api/events
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents(CancellationToken cancellationToken)
    {
        var events = await _eventService.GetAllEventsAsync(cancellationToken);
        return Ok(events);
    }

    // GET: api/events/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEvent(int id, CancellationToken cancellationToken)
    {
        var @event = await _eventService.GetEventByIdAsync(id, cancellationToken);
        if (@event == null)
            return NotFound();

        return Ok(@event);
    }

    // POST: api/events
    [HttpPost]
    public async Task<ActionResult<Event>> CreateEvent(Event @event, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdEvent = await _eventService.CreateEventAsync(@event, cancellationToken);

        return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
    }

    // DELETE: api/events/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id, CancellationToken cancellationToken)
    {
        var result = await _eventService.DeleteEventAsync(id, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}