using System.ComponentModel.DataAnnotations;

namespace Unyo.Dtos;

// GET
public record RegistrationDto(
    int Id,
    string ParticipantName,
    DateTime RegistrationDate,
    int TicketId,
    string TicketName,
    decimal TicketPrice,
    string EventTitle,
    DateTime EventDate);

// POST
public record CreateRegistrationDto(
    [Required] int TicketId,
    [Required, StringLength(100, MinimumLength = 2)] string ParticipantName);