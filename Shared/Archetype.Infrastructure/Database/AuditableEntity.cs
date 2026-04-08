namespace Archetype.Infrastructure.Database;

public abstract class AuditableEntity<TKey> : IAuditableEntity
    where TKey : notnull
{
    [
        Key,
        DatabaseGenerated(DatabaseGeneratedOption.Identity),
        Column(Order = 0)
    ]
    public TKey Id { get; set; } = default!;

    [
        DatabaseGenerated(DatabaseGeneratedOption.Identity),
        Column(Order = 996)
    ]
    public Guid Uuid { get; set; }

    [
        Required,
        Column(TypeName = "datetime2(7)", Order = 997)
    ]
    public DateTime Created { get; set; }

    [
        MaxLength(50),
        Column(TypeName = "varchar(50)", Order = 998)
    ]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime2(7)", Order = 999)]
    public DateTime? LastModified { get; set; }

    [
        MaxLength(50),
        Column(TypeName = "varchar(50)", Order = 1000)
    ]
    public string? LastModifiedBy { get; set; }
}