using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unyo.Services;
using Unyo.Dtos;
using Unyo.Mappings;

namespace Unyo.Controllers.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenuesController(IVenueService venueService)
    {
        _venueService = venueService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<VenueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenues(CancellationToken cancellationToken)
    {
        var venues = await _venueService.GetAllVenuesAsync(cancellationToken);
        return Ok(venues.Select(v => v.MapToDto()));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(VenueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VenueDto>> GetVenue(int id, CancellationToken cancellationToken)
    {
        var venue = await _venueService.GetVenueByIdAsync(id, cancellationToken);
        if (venue == null) return NotFound();
        return Ok(venue.MapToDto());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Vendor")]
    [ProducesResponseType(typeof(VenueDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VenueDto>> CreateVenue([FromBody] CreateVenueDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var venue = dto.MapToEntity();
        var createdVenue = await _venueService.CreateVenueAsync(venue, cancellationToken);
        return CreatedAtAction(nameof(GetVenue), new { id = createdVenue.Id }, createdVenue.MapToDto());
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Vendor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVenue(int id, CancellationToken cancellationToken)
    {
        var result = await _venueService.DeleteVenueAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}