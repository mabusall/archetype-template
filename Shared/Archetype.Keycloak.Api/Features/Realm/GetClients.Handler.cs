namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmGetRealmsQueryHandler(KeycloakApiClient keycloakApiClient)
    : IQueryHandler<KeycloakRealmGetRealmsQuery, IList<KeycloakRealmGetRealmsResult>>
{
    public async Task<IList<KeycloakRealmGetRealmsResult>> Handle(KeycloakRealmGetRealmsQuery query, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.GetAllClientsAsync(cancellationToken);
}