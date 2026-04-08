namespace Archetype.Keycloak.Api.Features.Admin;

internal class KeycloakAdminLogoutCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakAdminLogoutCommand, KeycloakSuccessStatusResult>
{
    public async Task<KeycloakSuccessStatusResult> Handle(KeycloakAdminLogoutCommand command, CancellationToken cancellationToken)
    {
        return await keycloakApiClient.Admin.LogoutAsync(command.RefreshToken, cancellationToken);
    }
}