namespace Archetype.Shared.Configuration;

public sealed class VaultSettingOptions
{
    public static string ConfigurationKey => "Vault";

    /// <summary>Enable or disable Vault integration completely.</summary>
    public bool IsEnabled { get; init; }

    /// <summary>Vault base URL, e.g. "http://localhost:8200".</summary>
    public required string AddressUri { get; init; }

    /// <summary>KV mount point name. For your dev setup it's "secret".</summary>
    public required string MountPoint { get; init; }

    /// <summary>
    /// Gets the name of the environment variable used for configuration or lookup.
    /// </summary>
    public required string EnvironmentVarName { get; init; }

    /// <summary>
    /// List of KV paths to load and merge into configuration,
    /// e.g. "flixhub/dev/appsettings".
    /// Later entries override earlier ones.
    /// </summary>
    public string[] VaultSecretsPathsList { get; init; } = [];
}
