using Unyo.Models;
namespace Unyo.Repositories;


public interface IRepository<T> where T : BaseEntity // This constraint ensures that T must have an Id property.
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
