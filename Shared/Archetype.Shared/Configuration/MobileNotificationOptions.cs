namespace Archetype.Shared.Configuration;

public class MobileNotificationOptions
{
    public const string ConfigurationKey = "MobileNotification";

    public bool IsActive { get; set; }
    public bool UseConsumer { get; set; }
    public FirebaseServiceAccountConfig FirebaseConfiguration { get; set; } = default!;
    public AndroidConfiguration AndroidConfiguration { get; set; } = default!;
}

public class FirebaseServiceAccountConfig
{
    [JsonPropertyName("type")]
    public string? CredentialType { get; set; }

    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    [JsonPropertyName("private_key_id")]
    public string? PrivateKeyId { get; set; }

    [JsonPropertyName("private_key")]
    public string? PrivateKey { get; set; }

    [JsonPropertyName("client_email")]
    public string? ClientEmail { get; set; }

    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    [JsonPropertyName("auth_uri")]
    public string? AuthUri { get; set; }

    [JsonPropertyName("token_uri")]
    public string? TokenUri { get; set; }

    [JsonPropertyName("auth_provider_x509_cert_url")]
    public string? AuthProviderX509CertUrl { get; set; }

    [JsonPropertyName("client_x509_cert_url")]
    public string? ClientX509CertUrl { get; set; }

    [JsonPropertyName("universe_domain")]
    public string? UniverseDomain { get; set; }
}

public class AndroidConfiguration
{
    public string? ChannelId { get; set; }
    public string? ClickAction { get; set; }
}