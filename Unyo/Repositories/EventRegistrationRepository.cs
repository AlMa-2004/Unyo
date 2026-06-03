using Microsoft.EntityFrameworkCore;
using Unyo.Models;
using Unyo.Data;

namespace Unyo.Repositories;

public class EventRegistrationRepository : Repository<EventRegistration>, IEventRegistrationRepository
{
    public EventRegistrationRepository(AppDbContext context) : base(context) { }

    public async Task<List<EventRegistration>> GetHistoryByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.EventRegistrations
            .Where(r => r.UserId != null && r.UserId == userId)
            .Include(r => r.Ticket)
                .ThenInclude(t => t.Event)
                    .ThenInclude(e => e!.Venue)
            .ToListAsync(cancellationToken);
    }
}