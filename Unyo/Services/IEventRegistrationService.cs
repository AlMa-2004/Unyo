using Unyo.Models;

namespace Unyo.Services;

public interface IEventRegistrationService
{
    Task<IEnumerable<EventRegistration>> GetUserRegistrationHistoryAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> RegisterUserToEventAsync(EventRegistration registration, CancellationToken cancellationToken = default);
}