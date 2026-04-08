namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmGetUserQueryHandler(KeycloakApiClient keycloakApiClient)
    : IQueryHandler<KeycloakRealmGetUserQuery, IList<KeycloakRealmGetUserResult>>
{
    public async Task<IList<KeycloakRealmGetUserResult>> Handle(KeycloakRealmGetUserQuery query, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.GetUserAsync(query, cancellationToken);
}