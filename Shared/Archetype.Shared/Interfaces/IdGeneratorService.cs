namespace Archetype.Shared.Interfaces;

public class IdGeneratorService : IIdGeneratorService
{
    /// <summary>
    /// Header / context key used to store the correlation id.
    /// </summary>
    public static string CorrelationIdKey => "SystemCorrelationId";

    public Guid NewID() => Guid.CreateVersion7(); // NewId.NextGuid();
}
