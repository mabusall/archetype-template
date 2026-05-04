namespace Archetype.Infrastructure.Services;

// Keep index in cache so it survives service lifetimes (if you keep the same container)
public sealed class TokenRing(IDistributedCache cache,
                              string cacheKey,
                              IList<string> tokens)
    : ITokenRing
{
    private readonly Lock _lock = new();

    private int Index
    {
        get
        {
            // int is a value type, but the cache interface requires reference types.
            // Store as string and parse, or use a boxed int (int?).
            string? cached = cache.GetStringAsync(cacheKey, CancellationToken.None).Result;
            if (cached is not null && int.TryParse(cached, out int idx))
                return idx;
            return 0;
        }
        set
        {
            // Store as string to satisfy the reference type constraint.
            cache.SetStringAsync(cacheKey,
                                 value.ToString(),
                                 new DistributedCacheEntryOptions(),
                                 CancellationToken.None);
        }
    }

    public string Current
    {
        get
        {
            int idx = Index % tokens.Count;
            return tokens[idx];
        }
    }

    public string PeekNext()
    {
        int idx = (Index + 1) % tokens.Count;
        return tokens[idx];
    }

    public void Advance()
    {
        lock (_lock)
        {
            int next = (Index + 1) % tokens.Count;
            Index = next;
        }
    }

    public void SetIndex(int index)
    {
        lock (_lock)
        {
            Index = ((index % tokens.Count) + tokens.Count) % tokens.Count;
        }
    }
}