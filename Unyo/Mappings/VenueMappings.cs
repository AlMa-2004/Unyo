using Unyo.Models;
using Unyo.Dtos;

namespace Unyo.Mappings;

public static class VenueMappings
{
    public static VenueDto MapToDto(this Venue venue) =>
        venue == null ? null! : new VenueDto(
            venue.Id,
            venue.Name,
            venue.Address,
            venue.Capacity,
            venue.ImagePath
        );
    public static Venue MapToEntity(this CreateVenueDto dto) =>
        dto == null ? null! : new Venue
        {
            Name = dto.Name,
            Address = dto.Address,
            Capacity = dto.Capacity,
            ImagePath = dto.ImagePath
        };
}