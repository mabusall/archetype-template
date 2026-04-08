namespace Archetype.Infrastructure.Database;

public interface IReadOnlyUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Gets the underlying Entity Framework database context associated with this instance.
    /// </summary>
    /// <remarks>Use the returned <see cref="DbContext"/> to perform advanced data operations or to access
    /// features not exposed by higher-level abstractions. The caller is responsible for managing the context's lifetime
    /// if it is disposed outside of this instance.</remarks>
    /// <returns>A <see cref="DbContext"/> representing the current database context.</returns>
    DbContext Context();
}