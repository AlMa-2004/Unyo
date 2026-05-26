namespace Unyo.Models;

public class EventRegistration
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public int EventId { get; set; }
    public int TicketId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public Event Event { get; set; } = null!;
    public Ticket Ticket { get; set; } = null!;
}