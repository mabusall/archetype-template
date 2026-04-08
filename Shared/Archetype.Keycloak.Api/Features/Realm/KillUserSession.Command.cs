namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmKillUserSessionCommand
(
    Guid SessionId
) : ICommand<KeycloakSuccessStatusResult>;