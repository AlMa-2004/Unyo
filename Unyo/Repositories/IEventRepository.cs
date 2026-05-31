using Unyo.Models;

namespace Unyo.Repositories;

public interface IEventRepository : IRepository<Event>
{
    Task<List<Event>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<Event?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}