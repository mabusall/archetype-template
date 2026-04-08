namespace Archetype.Infrastructure.Database;

public class ReadOnlyUnitOfWork(DbContext context) : IReadOnlyUnitOfWork
{
    public DbContext Context() => context;

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}