namespace Archetype.Shared.Configuration;

public class RedisOptions
{
    public static string ConfigurationKey => "Redis";

    public required string AddressUri { get; set; }
    public required string Password { get; set; }
    public required string OutputCachePrefix { get; set; }
    public required string DistributedCachePrefix { get; set; }
    public bool IsEnabled { get; set; } = false;
    public bool SecureSsl { get; set; } = false;
}
