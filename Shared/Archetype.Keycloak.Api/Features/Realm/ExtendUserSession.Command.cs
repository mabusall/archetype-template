namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmExtendUserSessionCommand
(
    string RefreshToken
) : ICommand<KeycloakSuccessStatusResult>;