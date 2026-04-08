namespace Archetype.Keycloak.Api.Features.Admin;

internal class KeycloakAdminClientsQueryHandler(KeycloakApiClient keycloakApiClient)
    : IQueryHandler<KeycloakAdminClientsQuery, IList<KeycloakAdminClientsResult>>
{
    public async Task<IList<KeycloakAdminClientsResult>> Handle(KeycloakAdminClientsQuery query, CancellationToken cancellationToken)
    {
        return await keycloakApiClient.Admin.GetAllClientsAsync(cancellationToken);
    }
}