namespace Archetype.Infrastructure.Database;

public class ReadOnlyRepository<TEntity, TBrief>(DbContext context)
    : IReadOnlyRepository<TEntity, TBrief>
    where TEntity : class
    where TBrief : class
{
    private readonly DbSet<TEntity> _table = context.Set<TEntity>();

    #region [ database query ]

    public IQueryable<TEntity> AsQueryable() => _table.AsNoTracking();

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken)
        => AsQueryable().AnyAsync(expression, cancellationToken);

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken)
        => AsQueryable().CountAsync(expression, cancellationToken);

    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken)
        => AsQueryable().Where(expression).ToListAsync(cancellationToken);

    public async Task<PaginatedList<TBrief>> GetPaginatedListAsync(IQueryable<TEntity> query,
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

        // DB-side projection to TEntity
        List<TEntity> items = await tableEntity
            .OrderBy(sortBy)
            .Skip((pageNumberCriteria - 1) * pageSizeCriteria)
            .Take(pageSizeCriteria)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new PaginatedList<TBrief>(items.Adapt<List<TBrief>>(),
                                         count,
                                         pageNumberCriteria,
                                         pageSizeCriteria);
    }

    #endregion
}