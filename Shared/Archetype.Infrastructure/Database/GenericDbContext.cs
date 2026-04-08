namespace Archetype.Infrastructure.Database;

public abstract class GenericDbContext(DbContextOptions options,
                                       IIdGeneratorService idGeneratorService,
                                       ICurrentUserService currentUserService)
    : DbContext(options)
{
    protected abstract string Schema { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrWhiteSpace(Schema))
        {
            modelBuilder.HasDefaultSchema(Schema);
        }

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        string userId = currentUserService.UserId ?? string.Empty;

        foreach (EntityEntry<IAuditableEntity> entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Uuid = entry.Entity.Uuid == Guid.Empty ? idGeneratorService.NewID() : entry.Entity.Uuid;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.Created = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModified = DateTime.UtcNow;
                    break;
            }

            // Handle concurrency
            if (entry.Entity is IConcurrencyAware concurrencyAwareEntity
                && (entry.State == EntityState.Added || entry.State == EntityState.Modified))
            {
                concurrencyAwareEntity.ConcurrencyStamp = Guid.NewGuid().ToString();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}