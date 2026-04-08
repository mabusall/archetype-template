namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmLoginCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakRealmLoginCommand, KeycloakRealmLoginResult>
{
    public async Task<KeycloakRealmLoginResult> Handle(KeycloakRealmLoginCommand command, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.Login(command, cancellationToken);
}