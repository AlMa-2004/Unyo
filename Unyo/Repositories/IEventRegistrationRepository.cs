using Unyo.Models;

namespace Unyo.Repositories;

public interface IEventRegistrationRepository : IRepository<EventRegistration>
{
    Task<List<EventRegistration>> GetHistoryByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}