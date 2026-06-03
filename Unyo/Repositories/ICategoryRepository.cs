using Unyo.Models;

namespace Unyo.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    // Empty interface for now, but can be extended with category-specific methods in the future.

    public Task<List<Category>> GetByIdsAsync(List<int> ids, CancellationToken ct);
}