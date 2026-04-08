namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmDeleteUserCommand
(
    Guid Id
) : ICommand<KeycloakSuccessStatusResult>;