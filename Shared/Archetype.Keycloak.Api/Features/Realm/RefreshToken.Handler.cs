namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmRefreshTokenCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakRealmRefreshTokenCommand, KeycloakRealmRefreshTokenResult>
{
    public async Task<KeycloakRealmRefreshTokenResult> Handle(KeycloakRealmRefreshTokenCommand command, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.RefreshTokenAsync(command, cancellationToken);
}