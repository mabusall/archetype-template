namespace Archetype.Keycloak.Api.Features.Admin;

internal class KeycloakAdminLoginCommandHandler(KeycloakApiClient keycloakApiClient)
    : ICommandHandler<KeycloakAdminLoginCommand, KeycloakAdminLoginResult>
{
    public async Task<KeycloakAdminLoginResult> Handle(KeycloakAdminLoginCommand command, CancellationToken cancellationToken)
    {
        return await keycloakApiClient.Admin.LoginAsync(cancellationToken);
    }
}