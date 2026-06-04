using System.ComponentModel.DataAnnotations;

namespace Unyo.Dtos;

// GET
public record EventDto(
    int Id,
    string Title,
    string Description,
    DateTime Date,
    string? ImagePath,
    string VenueName,
    List<string> CategoryNames,
    List<TicketSummaryDto> AvailableTickets,
    string UserId,
    int Capacity,    
    int CurrentRegistrations
)
{
    public int RemainingSeats => Capacity - CurrentRegistrations;
}

// POST
public record CreateEventDto(
    [Required, MinLength(5), MaxLength(200)] string Title,
    [Required, MinLength(20), MaxLength(2000)] string Description,
    [Required] DateTime Date,
    string? ImagePath,
    [Required] int VenueId,
    [Required] List<int> CategoryIds);

// PUT
public record UpdateEventDto(
    [Required, MinLength(5), MaxLength(200)] string Title,
    [Required, MinLength(20), MaxLength(2000)] string Description,
    [Required] DateTime Date,
    string? ImagePath,
    [Required] int VenueId,
    [Required] List<int> CategoryIds);

public record TicketSummaryDto(int Id, string Name, decimal Price);