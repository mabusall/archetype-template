namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmLogoutCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakRealmLogoutCommand, KeycloakSuccessStatusResult>
{
    public async Task<KeycloakSuccessStatusResult> Handle(KeycloakRealmLogoutCommand command, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.LogoutAsync(command, cancellationToken);
}