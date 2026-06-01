using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Unyo.Models;

public class EventRegistration : BaseEntity
{
    public string? UserId { get; set; }

    public int TicketId { get; set; }

    // In case an user gets deleted, keep the beneficiary name for reimbursement.
    [Required]
    [StringLength(100)]
    public string ParticipantName { get; set; } = string.Empty;

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Ticket Ticket { get; set; }

    [JsonIgnore]
    public User? User { get; set; }
}