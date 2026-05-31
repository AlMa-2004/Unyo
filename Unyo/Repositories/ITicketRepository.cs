using Unyo.Models;

namespace Unyo.Repositories;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<List<Ticket>> GetTicketsWithEventAsync(CancellationToken cancellationToken = default);
}