namespace Archetype.Shared.Configuration;

public class TokenRingOptions
{
    public string CacheKey { get; set; } = default!;

    public IList<string> Tokens { get; set; } = [];
}