namespace Archetype.Keycloak.Api.Features.Admin;

public record KeycloakAdminClientsQuery() : IQuery<IList<KeycloakAdminClientsResult>>;

public record KeycloakAdminClientsResult
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("clientId")]
    public string? ClientId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("rootUrl")]
    public string? RootUrl { get; set; }

    [JsonPropertyName("baseUrl")]
    public string? BaseUrl { get; set; }

    [JsonPropertyName("surrogateAuthRequired")]
    public bool SurrogateAuthRequired { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("alwaysDisplayInConsole")]
    public bool AlwaysDisplayInConsole { get; set; }

    [JsonPropertyName("clientAuthenticatorType")]
    public string? ClientAuthenticatorType { get; set; }

    [JsonPropertyName("redirectUris")]
    public string[]? RedirectUris { get; set; }

    [JsonPropertyName("bearerOnly")]
    public bool BearerOnly { get; set; }

    [JsonPropertyName("consentRequired")]
    public bool ConsentRequired { get; set; }

    [JsonPropertyName("standardFlowEnabled")]
    public bool StandardFlowEnabled { get; set; }

    [JsonPropertyName("implicitFlowEnabled")]
    public bool ImplicitFlowEnabled { get; set; }

    [JsonPropertyName("directAccessGrantsEnabled")]
    public bool DirectAccessGrantsEnabled { get; set; }

    [JsonPropertyName("serviceAccountsEnabled")]
    public bool ServiceAccountsEnabled { get; set; }

    [JsonPropertyName("publicClient")]
    public bool PublicClient { get; set; }

    [JsonPropertyName("frontchannelLogout")]
    public bool FrontchannelLogout { get; set; }

    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }

    [JsonPropertyName("fullScopeAllowed")]
    public bool FullScopeAllowed { get; set; }

    [JsonPropertyName("nodeReRegistrationTimeout")]
    public int NodeReRegistrationTimeout { get; set; }

    [JsonPropertyName("defaultClientScopes")]
    public string[]? DefaultClientScopes { get; set; }

    [JsonPropertyName("optionalClientScopes")]
    public string[]? OptionalClientScopes { get; set; }
}