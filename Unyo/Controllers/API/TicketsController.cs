using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unyo.Services;
using Unyo.Dtos;
using Unyo.Mappings;

namespace Unyo.Controllers.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<TicketDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets(CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetAllTicketsAsync(cancellationToken);
        return Ok(tickets.Select(t => t.MapToDto()));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> GetTicket(int id, CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id, cancellationToken);
        if (ticket == null) return NotFound();
        return Ok(ticket.MapToDto());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Vendor")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var ticket = dto.MapToEntity();
        var createdTicket = await _ticketService.CreateTicketAsync(ticket, cancellationToken);
        return CreatedAtAction(nameof(GetTicket), new { id = createdTicket.Id }, createdTicket.MapToDto());
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Vendor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTicket(int id, CancellationToken cancellationToken)
    {
        var result = await _ticketService.DeleteTicketAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}