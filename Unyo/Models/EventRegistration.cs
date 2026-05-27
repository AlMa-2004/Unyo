namespace Unyo.Models;

public class EventRegistration : BaseIdentity
{
    public string UserId { get; set; } = string.Empty;
    public int TicketId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public Ticket Ticket { get; set; } = null!;
}