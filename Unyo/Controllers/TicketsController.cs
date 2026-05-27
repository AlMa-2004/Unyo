using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unyo.Models;
// using Unyo.Data;

namespace Unyo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TicketsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Tickets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets(CancellationToken cancellationToken)
    {
        return await _context.Tickets
            .Include(t => t.Event)
            .ToListAsync(cancellationToken);
    }

    // GET: api/Tickets/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(int id, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Event)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (ticket == null)
        {
            return NotFound();
        }

        return ticket;
    }

    // PUT: api/Tickets/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTicket(int id, Ticket incomingTicket, CancellationToken cancellationToken)
    {
        if (id != incomingTicket.Id)
        {
            return BadRequest();
        }

        var ticketFromDb = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (ticketFromDb == null)
        {
            return NotFound();
        }
     
        ticketFromDb.TypeName = incomingTicket.TypeName;
        ticketFromDb.Price = incomingTicket.Price;
        ticketFromDb.EventId = incomingTicket.EventId;

        try
        {    
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TicketExists(id))
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

    // POST: api/Tickets 
    [HttpPost]
    public async Task<ActionResult<Ticket>> PostTicket(Ticket incomingTicket, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ticket = new Ticket
        {
            TypeName = incomingTicket.TypeName,
            Price = incomingTicket.Price,
            EventId = incomingTicket.EventId
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction("GetTicket", new { id = ticket.Id }, ticket);
    }

    // DELETE: api/Tickets/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FindAsync(id, cancellationToken);
        if (ticket == null)
        {
            return NotFound();
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private bool TicketExists(int id)
    {
        return _context.Tickets.Any(e => e.Id == id);
    }
}