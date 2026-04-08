namespace Archetype.Infrastructure.Database;

public interface IReadOnlyRepository<TEntity, TBrief>
    where TEntity : class
    where TBrief : class
{
    #region [ database query ]

    /// <summary>
    /// Asynchronously retrieves a list of entities that satisfy the specified filter expression.
    /// </summary>
    /// <param name="expression">An expression used to filter the entities to be retrieved. Only entities for which the expression evaluates to
    /// <see langword="true"/> are included in the result.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of entities matching the
    /// filter expression. If no entities match, the list will be empty.</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a paginated list of brief entity representations from the specified query.
    /// </summary>
    /// <remarks>The method executes the query with paging applied and returns only the brief representations
    /// of each entity. Paging is zero-based; ensure that page numbers and sizes are within valid ranges to avoid empty
    /// results.</remarks>
    /// <param name="query">The source query used to select entities for pagination. Must not be null.</param>
    /// <param name="pageNumber">The page number to retrieve. If null, the first page is returned. Must be greater than zero if specified.</param>
    /// <param name="pageSize">The number of items per page. If null, a default page size is used. Must be greater than zero if specified.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see
    /// cref="PaginatedList{TBrief}"/> with the items for the specified page. The list will be empty if no items match
    /// the query.</returns>
    Task<PaginatedList<TBrief>> GetPaginatedListAsync(IQueryable<TEntity> query,
                                                      int? pageNumber,
                                                      int? pageSize,
                                                      CancellationToken cancellationToken);

    /// <summary>
    /// Returns this object typed as IQueryable of TEntity
    /// </summary>
    /// <param name="trackable">
    /// If the value is false, The change tracker will not track any of the entities that are returned from a LINQ query.
    /// Otherwise, The change tracker will keep track of changes for all entities that are returned.
    /// 
    /// Keep notice that if trackable = true will degrade the performance sometimes
    /// </param>
    /// <returns></returns>
    IQueryable<TEntity> AsQueryable();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression,
                        CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> expression,
                         CancellationToken cancellationToken);

    #endregion
}