using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unyo.Models;
//using Unyo.Data;

namespace Unyo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VenuesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public VenuesController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: api/Venues
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Venue>>> GetVenues(CancellationToken cancellationToken)
    {
        return await _context.Venues.ToListAsync(cancellationToken);
    }

    // GET: api/Venues/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Venue>> GetVenue(int id, CancellationToken cancellationToken)
    {
        var venue = await _context.Venues.FindAsync(id, cancellationToken);

        if (venue == null)
        {
            return NotFound();
        }

        return venue;
    }

    // PUT: api/Venues/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutVenue(int id, [FromForm] Venue incomingVenue, IFormFile? imageFile, CancellationToken cancellationToken)
    {
        if (id != incomingVenue.Id)
        {
            return BadRequest();
        }

        var venueFromDb = await _context.Venues
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (venueFromDb == null)
        {
            return NotFound();
        }

        venueFromDb.Name = incomingVenue.Name;
        venueFromDb.Address = incomingVenue.Address;
        venueFromDb.Capacity = incomingVenue.Capacity;

        if (imageFile != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var folderPath = Path.Combine(_env.WebRootPath, "images", "venues");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var savePath = Path.Combine(folderPath, fileName);
            using var stream = System.IO.File.Create(savePath);
            await imageFile.CopyToAsync(stream, cancellationToken);

            venueFromDb.ImagePath = $"/images/venues/{fileName}";
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VenueExists(id))
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

    // POST: api/Venues
    [HttpPost]
    public async Task<ActionResult<Venue>> PostVenue([FromForm] Venue incomingVenue, IFormFile? imageFile, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var venue = new Venue
        {
            Name = incomingVenue.Name,
            Address = incomingVenue.Address,
            Capacity = incomingVenue.Capacity
        };

        if (imageFile != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var folderPath = Path.Combine(_env.WebRootPath, "images", "venues");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var savePath = Path.Combine(folderPath, fileName);
            using var stream = System.IO.File.Create(savePath);
            await imageFile.CopyToAsync(stream, cancellationToken);

            venue.ImagePath = $"/images/venues/{fileName}";
        }

        _context.Venues.Add(venue);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction("GetVenue", new { id = venue.Id }, venue);
    }

    // DELETE: api/Venues/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVenue(int id, CancellationToken cancellationToken)
    {
        var venue = await _context.Venues.FindAsync(id, cancellationToken);
        if (venue == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(venue.ImagePath))
        {
            var fileDiskPath = Path.Combine(_env.WebRootPath, venue.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(fileDiskPath))
            {
                System.IO.File.Delete(fileDiskPath);
            }
        }

        _context.Venues.Remove(venue);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private bool VenueExists(int id)
    {
        return _context.Venues.Any(e => e.Id == id);
    }
}