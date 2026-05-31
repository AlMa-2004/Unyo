using Unyo.Models;

namespace Unyo.Repositories;

public interface IVenueRepository : IRepository<Venue>
{
    Task<Venue?> GetByIdWithEventsAsync(int id, CancellationToken cancellationToken = default);
}