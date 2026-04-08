namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmGetUserSessionsQueryHandler(KeycloakApiClient keycloakApiClient)
    : IQueryHandler<KeycloakRealmGetUserSessionsQuery, IList<KeycloakRealmGetUserSessionsResult>>
{
    public async Task<IList<KeycloakRealmGetUserSessionsResult>> Handle(KeycloakRealmGetUserSessionsQuery query, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.GetUserSessionsAsync(query, cancellationToken);
}