using Unyo.Models;
using Unyo.Repositories;

namespace Unyo.Services;

public class EventRegistrationService : IEventRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventRegistrationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<EventRegistration>> GetUserRegistrationHistoryAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.EventRegistrations.GetHistoryByUserIdAsync(userId, cancellationToken);
    }

    public async Task<bool> RegisterUserToEventAsync(EventRegistration registration, CancellationToken cancellationToken = default)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(registration.TicketId, cancellationToken);
        if (ticket == null) return false;

        registration.RegistrationDate = DateTime.UtcNow;

        await _unitOfWork.EventRegistrations.AddAsync(registration, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}