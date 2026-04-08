namespace Archetype.Keycloak.Api.Features.Realm;

public record KeycloakRealmGetUserQuery
(
    string Email
) : IQuery<IList<KeycloakRealmGetUserResult>>;

public record KeycloakRealmGetUserResult
{
    public Guid Id { get; set; }
    [
        JsonConverter(typeof(UnixTimestampToDateTimeConverter)),
        JsonPropertyName("createdTimeStamp")
    ]
    public DateTime CreatedDate { get; set; }
    public string? UserName { get; set; }
    [JsonPropertyName("enabled")]
    public bool IsActive { get; set; }
    [JsonPropertyName("emailVerified")]
    public bool IsVerified { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
};