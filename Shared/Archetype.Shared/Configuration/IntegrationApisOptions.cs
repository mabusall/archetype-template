namespace Archetype.Shared.Configuration;

public record IntegrationApisOptions
{
    public static string ConfigurationKey => "IntegrationApis";

    public Dictionary<string, IntegrationApi> Apis { get; set; } = [];
}

public record IntegrationApi
{
    public required string AddressUri { get; set; }
    public IList<string> Tokens { get; set; } = [];
    public string ResourcesUri { get; set; } = default!;
    public int Limit { get; set; }
    public IList<string> Quota { get; set; } = [];
}