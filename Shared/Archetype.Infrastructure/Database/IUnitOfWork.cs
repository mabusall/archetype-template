namespace Archetype.Infrastructure.Database;

public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Returns the underlying <see cref="DbContext"/> instance used for database operations.
    /// </summary>
    /// <remarks>The returned <see cref="DbContext"/> may be shared or managed by the implementing class.
    /// Changes made through the context may affect the state of tracked entities.</remarks>
    /// <returns>The <see cref="DbContext"/> associated with this object. The returned context can be used to query and update
    /// entities in the database.</returns>
    DbContext Context();

    /// <summary>
    /// Asynchronously saves all changes made in the context to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the save operation.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries
    /// written to the data store.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Clears all tracked changes, resetting the change tracker to its initial state.
    /// </summary>
    /// <remarks>Call this method to discard any pending changes that have been tracked but not yet committed.
    /// After invoking this method, the change tracker will no longer report any modifications until new changes are
    /// made.</remarks>
    void ClearChangeTracker();
}