namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmKillUserSessionCommand
(
    string Email
) : ICommand<KeycloakSuccessStatusResult>;