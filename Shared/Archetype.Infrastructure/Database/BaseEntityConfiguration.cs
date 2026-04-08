namespace Archetype.Infrastructure.Database;

/// <summary>
/// Base configuration for entities that inherit from AuditableEntity.
/// Configures common audit properties: Id, Created, CreatedBy, LastModified, LastModifiedBy.
/// </summary>
/// <typeparam name="TEntity">The entity type that inherits from AuditableEntity</typeparam>
/// <typeparam name="TKey">The primary key type</typeparam>
public abstract class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditableEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Configure common audit properties with comments
        builder.Property(p => p.Id)
            .HasComment("Primary key of the entity.");

        builder.Property(p => p.Uuid)
            .HasComment("Unique identifier (GUID) for external references and API queries.");

        builder.Property(p => p.Created)
            .HasComment("Date and time when the record was created.");

        builder.Property(p => p.CreatedBy)
            .HasComment("Username or identifier of the user who created the record.");

        builder.Property(p => p.LastModified)
            .HasComment("Date and time when the record was last modified.");

        builder.Property(p => p.LastModifiedBy)
            .HasComment("Username or identifier of the user who last modified the record.");

        // Configure indexes for commonly queried columns
        // Uuid - Frequently queried by external clients (e.g., GET /order/{uuid})
        builder.HasIndex(p => p.Uuid)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_Uuid")
            .IsUnique(); // Uuid should be unique across the table

        // Created - Used for sorting (e.g., ORDER BY Created DESC) and filtering (e.g., WHERE Created > @date)
        builder.HasIndex(p => p.Created)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_Created");
    }
}
