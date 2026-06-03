using Unyo.Models;
using Unyo.Repositories;

namespace Unyo.Services;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Events.GetAllWithDetailsAsync(cancellationToken);
    }

    public async Task<Event?> GetEventByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Events.GetByIdWithDetailsAsync(id, cancellationToken);
    }

    public async Task<Event> CreateEventAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Events.AddAsync(@event, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return @event;
    }

    public async Task<bool> DeleteEventAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingEvent = await _unitOfWork.Events.GetByIdAsync(id, cancellationToken);
        if (existingEvent == null) return false;

        _unitOfWork.Events.Delete(existingEvent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task UpdateEventCategoriesAsync(int eventId, List<int> categoryIds, CancellationToken cancellationToken = default)
    {
        var @event = await _unitOfWork.Events.GetByIdWithDetailsAsync(eventId, cancellationToken);
        if (@event == null) throw new Exception("Evenimentul nu a fost găsit.");

        var selectedCategories = await _unitOfWork.Categories.GetByIdsAsync(categoryIds, cancellationToken);

        @event.Categories = selectedCategories.ToList();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}