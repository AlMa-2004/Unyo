using System.ComponentModel.DataAnnotations;

namespace Unyo.Dtos;

public record VenueDto(int Id, string Name, string Address);

public record CreateVenueDto(
    [Required, MinLength(3), MaxLength(150)] string Name,
    [Required, MinLength(5), MaxLength(250)] string Address);