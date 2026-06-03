using Microsoft.EntityFrameworkCore;
using Unyo.Models;
using Unyo.Data;

namespace Unyo.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context) { }

    public async Task<List<Event>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(e => e.Categories)
            .Include(e => e.Venue)
            .Include(e => e.Tickets)
            .ToListAsync(cancellationToken);
    }

    public async Task<Event?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(e => e.Categories)
            .Include(e => e.Venue)
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}