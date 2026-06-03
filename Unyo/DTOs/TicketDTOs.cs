using System.ComponentModel.DataAnnotations;

namespace Unyo.Dtos;

public record TicketDto(int Id, string TypeName, decimal Price, int EventId, string EventTitle);

public record CreateTicketDto(
    [Required, MinLength(2), MaxLength(100)] string TypeName,
    [Required, Range(0.01, 10000.00)] decimal Price,
    [Required] int EventId);