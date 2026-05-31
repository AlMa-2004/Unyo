using Microsoft.AspNetCore.Mvc;
using Unyo.Models;
using Unyo.Services;

namespace Unyo.Controllers;

[ApiController]
[Route("api/[controller]")] // Endpoint: api/venues
public class VenuesController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenuesController(IVenueService venueService)
    {
        _venueService = venueService;
    }

    // GET: api/venues
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Venue>>> GetVenues(CancellationToken cancellationToken)
    {
        var venues = await _venueService.GetAllVenuesAsync(cancellationToken);
        return Ok(venues);
    }

    // GET: api/venues/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Venue>> GetVenue(int id, CancellationToken cancellationToken)
    {
        var venue = await _venueService.GetVenueByIdAsync(id, cancellationToken);
        if (venue == null)
            return NotFound();

        return Ok(venue);
    }

    // POST: api/venues
    [HttpPost]
    public async Task<ActionResult<Venue>> CreateVenue(Venue venue, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdVenue = await _venueService.CreateVenueAsync(venue, cancellationToken);
        return CreatedAtAction(nameof(GetVenue), new { id = createdVenue.Id }, createdVenue);
    }

    // DELETE: api/venues/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVenue(int id, CancellationToken cancellationToken)
    {
        var result = await _venueService.DeleteVenueAsync(id, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}