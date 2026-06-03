using Unyo.Models;

namespace Unyo.Services;

public interface IEventService
{
    Task<IEnumerable<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default);
    Task<Event?> GetEventByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Event> CreateEventAsync(Event @event, CancellationToken cancellationToken = default);
    Task<bool> DeleteEventAsync(int id, CancellationToken cancellationToken = default);

    Task UpdateEventCategoriesAsync(int eventId, List<int> categoryIds, CancellationToken cancellationToken = default);
    Task UpdateEventAsync(Event eventEntity, CancellationToken cancellationToken = default);
}