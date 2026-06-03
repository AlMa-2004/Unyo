using System.ComponentModel.DataAnnotations;
namespace Unyo.Models;

public class Event : BaseEntity
{
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    public string? ImagePath { get; set; }

    // Foreign Keys
    public int VenueId { get; set; }
    public Venue Venue { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    // One event can have multiple ticket types (VIP, Normal, etc.)
    public List<Ticket> Tickets { get; set; } = [];
}