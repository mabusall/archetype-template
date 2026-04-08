namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmGetRealmsQuery() : IQuery<IList<KeycloakRealmGetRealmsResult>>;

public record KeycloakRealmGetRealmsResult
{
    public Guid Id { get; set; } // Represents the Realm ID (UUID)
    public string? RealmId { get; set; } // Realm ID (e.g., "spp-cli")
    public string? Name { get; set; } // Name of the Realm
    public string? Description { get; set; } // Description of the Realm
    public bool SurrogateAuthRequired { get; set; } // Flag indicating if surrogate auth is required
    public bool Enabled { get; set; } // Flag indicating if the Realm is enabled
    public bool AlwaysDisplayInConsole { get; set; } // Flag to always display Realm in the console
    public string? RealmAuthenticatorType { get; set; } // Type of authentication (e.g., "Realm-secret")
    public string? Secret { get; set; } // Realm secret (if using Realm secret authentication)
    public string[]? RedirectUris { get; set; } // List of redirect URIs
    public string[]? WebOrigins { get; set; } // List of allowed web origins
    public int NotBefore { get; set; } // Not before timestamp (typically used in token validation)
    public bool BearerOnly { get; set; } // Flag indicating if the Realm is bearer-only
    public bool ConsentRequired { get; set; } // Flag indicating if consent is required
    public bool StandardFlowEnabled { get; set; } // Flag indicating if the standard flow (authorization code) is enabled
    public bool ImplicitFlowEnabled { get; set; } // Flag indicating if the implicit flow is enabled
    public bool DirectAccessGrantsEnabled { get; set; } // Flag indicating if direct access grants are enabled
    public bool ServiceAccountsEnabled { get; set; } // Flag indicating if service accounts are enabled
    public bool AuthorizationServicesEnabled { get; set; } // Flag indicating if authorization services are enabled
    public bool PublicRealm { get; set; } // Flag indicating if the Realm is a public Realm
    public bool FrontchannelLogout { get; set; } // Flag indicating if frontchannel logout is enabled
    public string? Protocol { get; set; } // The protocol used (e.g., "openid-connect")
}