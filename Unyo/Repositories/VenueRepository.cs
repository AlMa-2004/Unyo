using Microsoft.EntityFrameworkCore;
using Unyo.Models;
using Unyo.Data;

namespace Unyo.Repositories;

public class VenueRepository : Repository<Venue>, IVenueRepository
{
    public VenueRepository(AppDbContext context) : base(context) { }

    public async Task<Venue?> GetByIdWithEventsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Venues
            .Include(v => v.Events)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }
}