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
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public EventsController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: api/Events
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents(CancellationToken cancellationToken)
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .ToListAsync(cancellationToken);
    }

    // GET: api/Events/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEvent(int id, CancellationToken cancellationToken)
    {
        var @event = await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (@event == null)
        {
            return NotFound();
        }

        return @event;
    }

    // PUT: api/Events/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEvent(int id, [FromForm] Event incomingEvent, IFormFile? imageFile, CancellationToken cancellationToken)
    {
        if (id != incomingEvent.Id)
        {
            return BadRequest();
        }

        var eventFromDb = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (eventFromDb == null)
        {
            return NotFound();
        }

        eventFromDb.Title = incomingEvent.Title;
        eventFromDb.Description = incomingEvent.Description;
        eventFromDb.Date = incomingEvent.Date;
        eventFromDb.VenueId = incomingEvent.VenueId;
        eventFromDb.CategoryId = incomingEvent.CategoryId;

        if (imageFile != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName); // We use a GUID to ensure a unique filename, and preserve the original extension
            var imagesFolder = Path.Combine(_env.WebRootPath, "images");

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var savePath = Path.Combine(imagesFolder, fileName);
            using var stream = System.IO.File.Create(savePath);
            await imageFile.CopyToAsync(stream, cancellationToken);

            eventFromDb.ImagePath = $"/images/{fileName}";
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EventExists(id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    // POST: api/Events
    [HttpPost]
    public async Task<ActionResult<Event>> PostEvent([FromForm] Event incomingEvent, IFormFile? imageFile, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var @event = new Event
        {
            Title = incomingEvent.Title,
            Description = incomingEvent.Description,
            Date = incomingEvent.Date,
            VenueId = incomingEvent.VenueId,
            CategoryId = incomingEvent.CategoryId
        };

        if (imageFile != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var imagesFolder = Path.Combine(_env.WebRootPath, "images");

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var savePath = Path.Combine(imagesFolder, fileName);
            using var stream = System.IO.File.Create(savePath);
            await imageFile.CopyToAsync(stream, cancellationToken);

            @event.ImagePath = $"/images/{fileName}";
        }

        _context.Events.Add(@event);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
    }

    // DELETE: api/Events/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id, CancellationToken cancellationToken)
    {
        var @event = await _context.Events.FindAsync(id, cancellationToken);
        if (@event == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(@event.ImagePath))
        {
            var fileDiskPath = Path.Combine(_env.WebRootPath, @event.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(fileDiskPath))
            {
                System.IO.File.Delete(fileDiskPath);
            }
        }

        _context.Events.Remove(@event);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private bool EventExists(int id)
    {
        return _context.Events.Any(e => e.Id == id);
    }
}