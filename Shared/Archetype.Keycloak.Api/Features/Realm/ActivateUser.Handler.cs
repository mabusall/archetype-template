namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmUpdateUserCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakRealmActivateUserCommand, KeycloakSuccessStatusResult>
{
    public async Task<KeycloakSuccessStatusResult> Handle(KeycloakRealmActivateUserCommand command, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.ActivateUserAsync(command, cancellationToken);
}
