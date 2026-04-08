namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmExtendUserSessionCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakRealmExtendUserSessionCommand, KeycloakSuccessStatusResult>
{
    public async Task<KeycloakSuccessStatusResult> Handle(KeycloakRealmExtendUserSessionCommand command, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.ExtendUserSessionAsync(command, cancellationToken);
}