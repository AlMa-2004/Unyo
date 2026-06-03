using Unyo.Models;
using Unyo.Dtos;

namespace Unyo.Mappings;

public static class EventMappings
{
    // Maps Event Entity -> EventDto (for GET requests)
    public static EventDto MapToDto(this Event @event)
    {
        if (@event == null) return null!;

        return new EventDto(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.Date,
            @event.ImagePath,
            @event.Venue?.Name ?? "Unknown Venue",
            @event.Categories?.Select(c => c.Name).ToList() ?? new List<string> { "Unknown Category" },
            @event.Tickets?.Select(t => new TicketSummaryDto(t.Id, t.TypeName, t.Price)).ToList() ?? []
        );
    }

    // Maps CreateEventDto -> Event Entity (for POST requests)
    public static Event MapToEntity(this CreateEventDto dto)
    {
        if (dto == null) return null!;

        return new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Date = dto.Date,
            ImagePath = dto.ImagePath,
            VenueId = dto.VenueId
        };
    }
}