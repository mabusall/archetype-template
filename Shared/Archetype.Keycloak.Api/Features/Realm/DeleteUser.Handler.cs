namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmDeleteUserCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakRealmDeleteUserCommand, KeycloakSuccessStatusResult>
{
    public async Task<KeycloakSuccessStatusResult> Handle(KeycloakRealmDeleteUserCommand command, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.DeleteUserAsync(command, cancellationToken);
}