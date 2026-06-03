using Microsoft.EntityFrameworkCore;
using Unyo.Data;
using Unyo.Models;

namespace Unyo.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<List<Category>> GetByIdsAsync(List<int> ids, CancellationToken ct)
    {
        return await _context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync(ct);
    }
}