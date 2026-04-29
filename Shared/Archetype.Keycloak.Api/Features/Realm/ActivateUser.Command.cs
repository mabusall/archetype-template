namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmActivateUserCommand
(
    string Email,
    bool Flag
) : ICommand<KeycloakSuccessStatusResult>;
