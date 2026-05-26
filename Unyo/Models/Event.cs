using System.ComponentModel.DataAnnotations;
namespace Unyo.Models; 

public class Event
{
    public int Id { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    // Foreign Keys
    public int VenueId { get; set; }
    public Venue Venue { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Relationship: One event has many registrations
    public List<EventRegistration> Registrations { get; set; } = [];
}