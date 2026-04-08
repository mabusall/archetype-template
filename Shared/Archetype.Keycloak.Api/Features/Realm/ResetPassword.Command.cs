namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmResetPasswordCommand
(
    string Email,
    string Password
) : ICommand<KeycloakSuccessStatusResult>;