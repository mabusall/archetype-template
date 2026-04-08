namespace Archetype.Infrastructure.Database;

public class UnitOfWork(DbContext context) : IUnitOfWork
{
    public DbContext Context() => context;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => context.SaveChangesAsync(cancellationToken);

    public void ClearChangeTracker()
        => context.ChangeTracker.Clear();

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}