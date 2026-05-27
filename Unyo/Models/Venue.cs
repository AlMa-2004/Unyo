using System.ComponentModel.DataAnnotations;

namespace Unyo.Models;


public class Venue : BaseIdentity
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Range(1, 100000, ErrorMessage = "Capacity must be at least 1.")]
    public int Capacity { get; set; }

    public String? ImagePath { get; set; }

    // Relationship: One venue hosts many events
    public List<Event> Events { get; set; } = [];
}