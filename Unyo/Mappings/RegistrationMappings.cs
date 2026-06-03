using Unyo.Models;
using Unyo.Dtos;

namespace Unyo.Mappings;

public static class RegistrationMappings
{
    // Maps EventRegistration Entity -> RegistrationDto (for GET requests)
    public static RegistrationDto MapToDto(this EventRegistration reg)
    {
        if (reg == null) return null!;

        return new RegistrationDto(
            reg.Id,
            reg.ParticipantName,
            reg.RegistrationDate,
            reg.TicketId,
            reg.Ticket?.TypeName ?? "Standard Ticket",
            reg.Ticket?.Price ?? 0,
            reg.Ticket?.Event?.Title ?? "Unknown Event",
            reg.Ticket?.Event?.Date ?? DateTime.MinValue
        );
    }

    // Maps CreateRegistrationDto -> EventRegistration Entity (for POST requests)
    public static EventRegistration MapToEntity(this CreateRegistrationDto dto)
    {
        if (dto == null) return null!;

        return new EventRegistration
        {
            TicketId = dto.TicketId,
            ParticipantName = dto.ParticipantName,
            RegistrationDate = DateTime.UtcNow
        };
    }
}