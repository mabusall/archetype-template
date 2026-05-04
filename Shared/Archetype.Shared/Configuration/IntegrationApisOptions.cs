namespace Archetype.Shared.Configuration;

public record IntegrationApisOptions
{
    public static string ConfigurationKey => "IntegrationApis";

    public Dictionary<string, IntegrationApi> Apis { get; set; } = [];
}

public record IntegrationApi
{
    public required string BaseUrl { get; set; }
    public IList<string> Tokens { get; set; } = [];
    public string ResourcesUrl { get; set; } = default!;
}