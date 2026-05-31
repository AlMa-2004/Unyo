using Microsoft.EntityFrameworkCore;
using Unyo.Models;
using Unyo.Data;

namespace Unyo.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context) { }

    public async Task<List<Ticket>> GetTicketsWithEventAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .Include(t => t.Event)
            .ToListAsync(cancellationToken);
    }
}