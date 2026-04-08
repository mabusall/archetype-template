namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmLogoutCommand(string RefreshToken)
    : ICommand<KeycloakSuccessStatusResult>;
