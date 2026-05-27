using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unyo.Models;
//using Unyo.Data;

namespace Unyo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventRegistrationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventRegistrationsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/EventRegistrations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventRegistration>>> GetEventsRegistrations(CancellationToken cancellationToken)
    {
        return await _context.EventsRegistrations
            .Include(r => r.Ticket)
                .ThenInclude(t => t.Event)
            .ToListAsync(cancellationToken);
    }

    // GET: api/EventRegistrations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EventRegistration>> GetEventRegistration(int id, CancellationToken cancellationToken)
    {
        var eventRegistration = await _context.EventsRegistrations
            .Include(r => r.Ticket)
                .ThenInclude(t => t.Event)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (eventRegistration == null)
        {
            return NotFound();
        }

        return eventRegistration;
    }

    // PUT: api/EventRegistrations/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEventRegistration(int id, EventRegistration incomingRegistration, CancellationToken cancellationToken)
    {
        if (id != incomingRegistration.Id)
        {
            return BadRequest();
        }

        var registrationFromDb = await _context.EventsRegistrations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (registrationFromDb == null)
        {
            return NotFound();
        }

        registrationFromDb.TicketId = incomingRegistration.TicketId;
        registrationFromDb.ParticipantName = incomingRegistration.ParticipantName;

        // registrationFromDb.UserId = incomingRegistration.UserId;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EventRegistrationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/EventRegistrations
    [HttpPost]
    public async Task<ActionResult<EventRegistration>> PostEventRegistration(EventRegistration incomingRegistration, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var eventRegistration = new EventRegistration
        {
            UserId = incomingRegistration.UserId,
            TicketId = incomingRegistration.TicketId,
            ParticipantName = incomingRegistration.ParticipantName,
            RegistrationDate = DateTime.UtcNow
        };

        _context.EventsRegistrations.Add(eventRegistration);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction("GetEventRegistration", new { id = eventRegistration.Id }, eventRegistration);
    }

    // DELETE: api/EventRegistrations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEventRegistration(int id, CancellationToken cancellationToken)
    {
        var eventRegistration = await _context.EventsRegistrations.FindAsync(id, cancellationToken);
        if (eventRegistration == null)
        {
            return NotFound();
        }

        _context.EventsRegistrations.Remove(eventRegistration);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private bool EventRegistrationExists(int id)
    {
        return _context.EventsRegistrations.Any(e => e.Id == id);
    }
}