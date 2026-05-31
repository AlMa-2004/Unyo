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
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .OrderBy(e => e.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<Event?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}