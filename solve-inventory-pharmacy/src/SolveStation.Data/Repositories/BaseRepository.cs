using Microsoft.EntityFrameworkCore;
using SolveStation.Common.Interfaces;
using SolveStation.Common.Models;
using SolveStation.Data.Context;

namespace SolveStation.Data.Repositories;

public abstract class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : notnull
{
    protected readonly PharmacyDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseRepository(PharmacyDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    // Alternative method names for compatibility
    public virtual async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(e => e.IsActive).ToListAsync(cancellationToken);
    }

    // Alternative method names for compatibility
    public virtual async Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            entity.Deactivate();
            DbSet.Update(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(e => e.Id.Equals(id) && e.IsActive, cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(e => e.IsActive, cancellationToken);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }

    protected virtual IQueryable<TEntity> GetActiveEntities()
    {
        return DbSet.Where(e => e.IsActive);
    }
}
