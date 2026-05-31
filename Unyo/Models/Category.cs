using System.ComponentModel.DataAnnotations;

namespace Unyo.Models;
public class Category : BaseEntity
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    // Relationship: One category has many events
    public List<Event> Events { get; set; } = [];
}