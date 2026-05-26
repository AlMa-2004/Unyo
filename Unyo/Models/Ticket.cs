using System.ComponentModel.DataAnnotations;

namespace Unyo.Models;

public class Ticket
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string TypeName { get; set; } = string.Empty; // These are not predetermined , they are created by the event organizer. Examples: "General Admission", "VIP", "Early Bird", etc.

    [Required]
    [Range(0.01, 10000.00)]
    public decimal Price { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
}