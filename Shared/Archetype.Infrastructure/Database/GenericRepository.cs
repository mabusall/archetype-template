namespace Archetype.Infrastructure.Database;

public class GenericRepository<TEntity, TKey, TBrief>(DbContext context)
    : IGenericRepository<TEntity, TKey, TBrief>
    where TEntity : AuditableEntity<TKey>
    where TKey : notnull
    where TBrief : class
{
    #region [ injected variables ]

    private readonly DbSet<TEntity> _table = context.Set<TEntity>();

    #endregion

    #region [ database query ]

    public IQueryable<TEntity> AsQueryable(bool trackable) =>
        trackable ?
        _table.AsTracking()
        :
        _table.AsNoTracking();

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        => await _table.FindAsync([id], cancellationToken);

    public Task<TEntity?> GetByUuidAsync(Guid uuid, CancellationToken cancellationToken)
        => AsQueryable(true).SingleOrDefaultAsync(e => e.Uuid == uuid, cancellationToken)!;

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken)
        => AsQueryable(false).AnyAsync(expression, cancellationToken);

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken)
        => AsQueryable(false).CountAsync(expression, cancellationToken);

    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression,
                                            CancellationToken cancellationToken)
        => AsQueryable(false).Where(expression).ToListAsync(cancellationToken);

    public async Task<PaginatedList<TBrief>> GetPaginatedListAsync(IQueryable<TEntity>? query,
                                                                   int? pageNumber,
                                                                   int? pageSize,
                                                                   string? sortBy,
                                                                   CancellationToken cancellationToken)
    {
        IQueryable<TEntity> tableEntity = query ?? _table.AsNoTracking();
        int pageNumberCriteria = pageNumber ?? 1;
        int pageSizeCriteria = pageSize ?? 10;
        int count = await tableEntity.CountAsync(cancellationToken);

        sortBy = string.IsNullOrWhiteSpace(sortBy) ? "Id ASC" : sortBy;


        // DB-side projection to TBrief
        List<TBrief> items = await tableEntity
            .OrderBy(sortBy)
            .Skip((pageNumberCriteria - 1) * pageSizeCriteria)
            .Take(pageSizeCriteria)
            .AsSplitQuery()
            .ProjectToType<TBrief>()
            .ToListAsync(cancellationToken);

        return new PaginatedList<TBrief>(items,
                                         count,
                                         pageNumberCriteria,
                                         pageSizeCriteria);
    }

    #endregion

    #region [ database context actions ]

    public TEntity Insert(TEntity entity)
        => _table.Add(entity).Entity;

    public void InsertRange(params TEntity[] entities)
        => _table.AddRange(entities);

    public void Update(TEntity entity)
        => _table.Update(entity);

    public void UpdateRange(params TEntity[] entities)
        => _table.UpdateRange(entities);

    public Task DeleteByQueryAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        => _table.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public void Delete(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _table.Remove(entity);
    }

    #endregion
}