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
    [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(CancellationToken cancellationToken)
    {
        var events = await _eventService.GetAllEventsAsync(cancellationToken);

        // Map list of entities to list of flat DTOs
        var dtos = events.Select(e => e.MapToDto());

        return Ok(dtos);
    }

    // GET: api/events/5
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetEvent(int id, CancellationToken cancellationToken)
    {
        var @event = await _eventService.GetEventByIdAsync(id, cancellationToken);
        if (@event == null)
            return NotFound();

        return Ok(@event.MapToDto());
    }

    // POST: api/events
    [HttpPost]
    [Authorize(Roles = "Admin,Vendor")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Convert incoming DTO to actual database Entity
        var @event = dto.MapToEntity();

        // Security rule: Vendors can only create events for themselves. 
        // Admins can create events for anyone (or pass a specific UserId via business logic if needed)
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin"))
        {
            @event.UserId = currentUserId ?? string.Empty;
        }
        else
        {
            // If admin creates it, we can default it to Admin's ID or handle it according to your core logic
            @event.UserId = currentUserId ?? string.Empty;
        }

        // Save via service
        var createdEvent = await _eventService.CreateEventAsync(@event, cancellationToken);

        //  Return 201 Created with the mapped Response DTO
        return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent.MapToDto());
    }

    // DELETE: api/events/5
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Vendor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEvent(int id, CancellationToken cancellationToken)
    {
        // Fetch the actual entity from DB first to inspect its ownership
        var @event = await _eventService.GetEventByIdAsync(id, cancellationToken);
        if (@event == null)
            return NotFound();

        // Validate authorization using your helper function (Crucial layer of security!)
        if (!IsEventOwnerOrAdmin(@event))
        {
            return Forbid();
        }

        // Execute deletion
        var result = await _eventService.DeleteEventAsync(id, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}