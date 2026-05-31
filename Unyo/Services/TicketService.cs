using Unyo.Models;
using Unyo.Repositories;

namespace Unyo.Services;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tickets.GetTicketsWithEventAsync(cancellationToken);
    }

    public async Task<Ticket?> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Ticket> CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Tickets.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ticket;
    }

    public async Task<bool> DeleteTicketAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;

        _unitOfWork.Tickets.Delete(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}