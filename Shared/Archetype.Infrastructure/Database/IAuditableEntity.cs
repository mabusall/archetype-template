namespace Archetype.Infrastructure.Database;

/// <summary>
/// Interface for entities that support auditing (tracking creation and modification)
/// </summary>
public interface IAuditableEntity
{
    Guid Uuid { get; set; }
    DateTime Created { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    string? LastModifiedBy { get; set; }
}
