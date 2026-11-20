using Enrollora.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Enrollora.Api.Repositories;

public sealed class Repository<TEntity>(ApplicationDbContext context)
    : IRepository<TEntity> where TEntity : class
{
    public IQueryable<TEntity> Query(bool tracking = false)
    {
        var query = context.Set<TEntity>().AsQueryable();
        return tracking ? query : query.AsNoTracking();
    }

    public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().FindAsync([id], cancellationToken).AsTask();

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);

    public void Remove(TEntity entity) => context.Set<TEntity>().Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
