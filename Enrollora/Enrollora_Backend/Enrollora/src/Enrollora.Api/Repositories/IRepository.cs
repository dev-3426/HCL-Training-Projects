using System.Linq.Expressions;

namespace Enrollora.Api.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query(bool tracking = false);
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
