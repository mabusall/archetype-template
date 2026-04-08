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
                                                                   CancellationToken cancellationToken)
    {
        IQueryable<TEntity> tableEntity = query ?? _table.AsNoTracking();
        int pageNumberCriteria = pageNumber is not null ? pageNumber.Value : 1;
        int pageSizeCriteria = pageSize is not null ? pageSize.Value : 10;

        int count = await tableEntity
            .TagWith("FORCE_LEGACY")
            .CountAsync(cancellationToken);

        List<TEntity> items = await tableEntity
            .Skip((pageNumberCriteria - 1) * pageSizeCriteria)
            .Take(pageSizeCriteria)
            .ToListAsync(cancellationToken);

        return new PaginatedList<TBrief>(items.Adapt<List<TBrief>>(),
                                         count,
                                         pageNumberCriteria,
                                         pageSizeCriteria);
    }

    #endregion
}