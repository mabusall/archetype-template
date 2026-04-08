namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmCreateUserCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakRealmCreateUserCommand, KeycloakSuccessStatusResult>
{
    public async Task<KeycloakSuccessStatusResult> Handle(KeycloakRealmCreateUserCommand command, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.CreateUserAsync(command, cancellationToken);
}