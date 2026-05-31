using Microsoft.AspNetCore.Mvc;
using Unyo.Models;
using Unyo.Services;

namespace Unyo.Controllers;

[ApiController]
[Route("api/[controller]")] // Endpoint: api/tickets
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    // GET: api/tickets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets(CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetAllTicketsAsync(cancellationToken);
        return Ok(tickets);
    }

    // GET: api/tickets/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(int id, CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id, cancellationToken);
        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }

    // POST: api/tickets
    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateTicket(Ticket ticket, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdTicket = await _ticketService.CreateTicketAsync(ticket, cancellationToken);
        return CreatedAtAction(nameof(GetTicket), new { id = createdTicket.Id }, createdTicket);
    }

    // DELETE: api/tickets/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id, CancellationToken cancellationToken)
    {
        var result = await _ticketService.DeleteTicketAsync(id, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}