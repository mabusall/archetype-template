namespace Archetype.Shared.Interfaces;

public interface IConcurrencyAware
{
    [ConcurrencyCheck, MaxLength(50)]
    public string ConcurrencyStamp { get; set; }
}
