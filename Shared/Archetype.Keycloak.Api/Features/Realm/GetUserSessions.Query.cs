namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmGetUserSessionsQuery
(
    string Email
) : IQuery<IList<KeycloakRealmGetUserSessionsResult>>;

public record KeycloakRealmGetUserSessionsResult
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public Guid UserId { get; set; }
    public string? IpAddress { get; set; }
    [JsonConverter(typeof(UnixTimestampToDateTimeConverter))]
    public DateTime Start { get; set; }
    [JsonConverter(typeof(UnixTimestampToDateTimeConverter))]
    public DateTime LastAccess { get; set; }
};