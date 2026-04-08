namespace Archetype.Infrastructure.Database;

public interface IGenericRepository<TEntity, TKey, TBrief>
    where TEntity : AuditableEntity<TKey>
    where TKey : notnull
    where TBrief : class
{
    #region [ database query ]

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity with the specified
    /// identifier, or null if no such entity exists.</returns>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier (UUID).
    /// </summary>
    /// <param name="uuid">The unique identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity that matches the
    /// specified UUID, or null if no such entity exists.</returns>
    Task<TEntity?> GetByUuidAsync(Guid uuid, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a list of entities that satisfy the specified filter expression.
    /// </summary>
    /// <param name="expression">An expression used to filter the entities to be retrieved. Only entities matching this condition are included in
    /// the result.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of entities that match the
    /// specified filter. The list is empty if no entities match.</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a paginated list of brief entity representations from the specified query, applying
    /// optional sorting and pagination parameters.
    /// </summary>
    /// <param name="query">The source query representing the collection of entities to paginate. Must not be null.</param>
    /// <param name="pageNumber">The one-based index of the page to retrieve. If null, the first page is returned.</param>
    /// <param name="pageSize">The maximum number of items to include in a single page. If null, a default page size is used.</param>
    /// <param name="sortBy">The name of the property to sort the results by. If null or empty, a default sort order is applied.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated list of brief entity
    /// representations for the specified page. The list will be empty if no items match the query.</returns>
    Task<PaginatedList<TBrief>> GetPaginatedListAsync(IQueryable<TEntity> query,
                                                      int? pageNumber,
                                                      int? pageSize,
                                                      string sortBy,
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
    IQueryable<TEntity> AsQueryable(bool trackable);

    /// <summary>
    /// Asynchronously determines whether any entities satisfy the specified condition.
    /// </summary>
    /// <param name="expression">An expression that defines the condition to test each entity against.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if any
    /// entities satisfy the condition; otherwise, <see langword="false"/>.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression,
                        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously counts the number of entities that satisfy the specified predicate.
    /// </summary>
    /// <param name="expression">An expression that defines the conditions to filter the entities to be counted.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous count operation. The task result contains the number of entities that
    /// match the specified predicate.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> expression,
                         CancellationToken cancellationToken);

    #endregion

    #region [ database context actions ]

    /// <summary>
    /// Inserts the specified entity into the data store.
    /// </summary>
    /// <param name="entity">The entity to insert. Cannot be null.</param>
    /// <returns>The inserted entity, potentially with updated values such as generated keys or timestamps.</returns>
    TEntity Insert(TEntity entity);

    /// <summary>
    /// Inserts the specified array of entities into the data store in a single operation.
    /// </summary>
    /// <param name="entities">An array of entities to insert. Cannot be null or contain null elements.</param>
    void InsertRange(params TEntity[] entities);

    /// <summary>
    /// Updates the specified entity in the data store.
    /// </summary>
    /// <param name="entity">The entity to update. Cannot be null.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Updates the specified entities in the data store.
    /// </summary>
    /// <param name="entities">An array of entities to update. Cannot be null or contain null elements.</param>
    void UpdateRange(params TEntity[] entities);

    /// <summary>
    /// Removes the specified entity from the data store.
    /// </summary>
    /// <param name="entity">The entity to remove. Cannot be null.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Asynchronously deletes all entities that match the specified predicate.
    /// </summary>
    /// <remarks>If no entities match the predicate, no deletions are performed. The operation is performed
    /// asynchronously and may be cancelled by passing a cancellation token.</remarks>
    /// <param name="predicate">An expression that defines the conditions used to select entities for deletion.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteByQueryAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    #endregion
}