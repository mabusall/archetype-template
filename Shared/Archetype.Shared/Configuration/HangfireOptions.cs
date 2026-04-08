namespace Archetype.Shared.Configuration;

public record HangfireOptions
{
    public static string ConfigurationKey => "Hangfire";

    public bool IsEnabled { get; set; }
    public required string SchemaName { get; set; }
    public required string Prefix { get; set; }
    public Dictionary<string, HangfireJob> Tasks { get; set; } = [];
}

public record HangfireJob
{
    public required string Id { get; set; }
    public bool IsEnabled { get; set; }
    public required string Handler { get; set; }
    public required bool AutoStart { get; set; }
    public TimeSpan Schedule { get; set; }
}