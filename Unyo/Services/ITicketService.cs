using Unyo.Models;

namespace Unyo.Services;

public interface ITicketService
{
    Task<IEnumerable<Ticket>> GetAllTicketsAsync(CancellationToken cancellationToken = default);
    Task<Ticket?> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Ticket> CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task<bool> DeleteTicketAsync(int id, CancellationToken cancellationToken = default);
}