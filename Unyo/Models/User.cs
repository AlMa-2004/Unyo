using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Unyo.Models;

public class User : IdentityUser
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public DateTime BirthDate { get; set; }

    public List<EventRegistration> Registrations { get; set; } = new();
}