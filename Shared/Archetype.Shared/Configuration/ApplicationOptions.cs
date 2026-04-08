namespace Archetype.Shared.Configuration;

public class ApplicationOptions
{
    public static string ConfigurationKey => "Application";

    public required string ApiKey { get; set; }
    public required string PrivateKey { get; set; }
    public required string PublicKey { get; set; }
    public string[] SupportedLanguages { get; set; } = [];
    public required string DbConnection { get; set; }
}