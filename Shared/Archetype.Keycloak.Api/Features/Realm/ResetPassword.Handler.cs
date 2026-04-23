namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmResetPasswordCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakRealmResetPasswordCommand, KeycloakSuccessStatusResult>
{
    public async Task<KeycloakSuccessStatusResult> Handle(KeycloakRealmResetPasswordCommand command, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.ResetPasswordAsync(command, cancellationToken);
}