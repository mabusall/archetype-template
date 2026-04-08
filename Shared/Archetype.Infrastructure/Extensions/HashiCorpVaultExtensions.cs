namespace Archetype.Infrastructure.Extensions;

public static class HashiCorpVaultExtensions
{
    private const string TokenEnvVarName = "VaultToken";

    /// <summary>
    /// Load secrets from HashiCorp Vault and merge them into the configuration.
    /// Usage from Program.cs:
    ///     await builder.Configuration.AddHashiCorpVaultAsync();
    /// </summary>
    public static async Task<IConfigurationBuilder> AddHashiCorpVaultAsync(this IConfigurationBuilder configurationBuilder,
                                                                           string vaultSectionName)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);
        ArgumentNullException.ThrowIfNull(vaultSectionName);

        // Build a temporary configuration snapshot to read Vault options.
        IConfigurationRoot tempConfig = configurationBuilder.Build();
        VaultSettingOptions options = tempConfig.GetSection(vaultSectionName).Get<VaultSettingOptions>()
            ?? throw new ArgumentException($"Configuration section '{vaultSectionName}' not found.",
                                           nameof(vaultSectionName));

        if (!options.IsEnabled)
        {
            // Explicitly disabled → do nothing, return builder unchanged.
            return configurationBuilder;
        }

        if (string.IsNullOrWhiteSpace(options.AddressUri))
        {
            throw new ArgumentException("Vault:Url is required when Vault is enabled.",
                                    nameof(vaultSectionName));
        }

        if (options.VaultSecretsPathsList is null ||
            options.VaultSecretsPathsList.Length == 0)
            throw new InvalidOperationException("Vault is enabled but Vault:VaultSecretsPathsList is empty.");

        string mountPoint = string.IsNullOrWhiteSpace(options.MountPoint)
            ? "secret"
            : options.MountPoint;

        string? token = tempConfig[TokenEnvVarName];
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Vault token is required but was not found in configuration.", nameof(vaultSectionName));

        VaultClient client = CreateVaultClient(options.AddressUri, token);

        foreach (string path in options.VaultSecretsPathsList)
        {
            if (string.IsNullOrWhiteSpace(path))
                continue;

            await FetchAndAddSecretAsync(client,
                                         path,
                                         mountPoint,
                                         configurationBuilder);
        }

        return configurationBuilder;
    }

    private static VaultClient CreateVaultClient(string url, string token)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Vault URL is null or empty.", nameof(url));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Vault token is null or empty.", nameof(token));

        TokenAuthMethodInfo authMethod = new(token);
        VaultClientSettings settings = new(url, authMethod);

        return new VaultClient(settings);
    }

    private static async Task FetchAndAddSecretAsync(IVaultClient client,
                                                     string secretPath,
                                                     string mountPoint,
                                                     IConfigurationBuilder configurationBuilder)
    {
        try
        {
            IKeyValueSecretsEngineV2 kv = client.V1.Secrets.KeyValue.V2;

            // secretPath example: "tasheer/dev/appsettings"
            Secret<SecretData> secret = await kv.ReadSecretAsync(secretPath, mountPoint: mountPoint);
            IDictionary<string, object>? data = secret.Data.Data; // IDictionary<string, object>

            if (data is null || data.Count == 0)
                throw new InvalidOperationException($"No data found at Vault path '{secretPath}'.");

            // Serialize data to JSON and add as a configuration source.
            string json = JsonSerializer.Serialize(data);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            configurationBuilder.AddJsonStream(new MemoryStream(bytes));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving Vault secret at path '{secretPath}': {ex.Message}",
                                                ex);
        }
    }
}
