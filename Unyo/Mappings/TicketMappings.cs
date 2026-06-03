using Unyo.Models;
using Unyo.Dtos;

namespace Unyo.Mappings;

public static class TicketMappings
{
    public static TicketDto MapToDto(this Ticket ticket) =>
        ticket == null ? null! : new TicketDto(
            ticket.Id,
            ticket.TypeName,
            ticket.Price,
            ticket.EventId ?? 0,
            ticket.Event?.Title ?? "Unknown Event");

    public static Ticket MapToEntity(this CreateTicketDto dto) =>
        dto == null ? null! : new Ticket { TypeName = dto.TypeName, Price = dto.Price, EventId = dto.EventId };
}