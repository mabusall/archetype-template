namespace Archetype.Keycloak.Api.Features.Realm;

internal class KeycloakRealmGetUserIntrospectQueryHandler(KeycloakApiClient keycloakApiClient)
    : IQueryHandler<KeycloakRealmGetUserIntrospectQuery, KeycloakRealmGetUserIntrospectResult>
{
    public async Task<KeycloakRealmGetUserIntrospectResult> Handle(KeycloakRealmGetUserIntrospectQuery query, CancellationToken cancellationToken)
        => await keycloakApiClient.Realm.GetUserIntrospectAsync(query, cancellationToken);
}