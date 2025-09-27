namespace SolveStation.Data.Repositories;

public interface IRepository<TEntity, TKey>
    where TEntity : class
{
    Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
