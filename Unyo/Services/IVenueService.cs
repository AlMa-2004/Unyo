using Unyo.Models;

namespace Unyo.Services;

public interface IVenueService
{
    Task<IEnumerable<Venue>> GetAllVenuesAsync(CancellationToken cancellationToken = default);
    Task<Venue?> GetVenueByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Venue> CreateVenueAsync(Venue venue, CancellationToken cancellationToken = default);
    Task<bool> DeleteVenueAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateVenueAsync(Venue venue, CancellationToken cancellationToken);
}