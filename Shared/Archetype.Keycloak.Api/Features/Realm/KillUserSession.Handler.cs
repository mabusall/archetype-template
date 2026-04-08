namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmKillUserSessionCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakRealmKillUserSessionCommand, KeycloakSuccessStatusResult>
{
    public async Task<KeycloakSuccessStatusResult> Handle(KeycloakRealmKillUserSessionCommand command, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.KillUserSessionAsync(command, cancellationToken);
}