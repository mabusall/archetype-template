namespace Archetype.Shared.Configuration;

public class KeycloakOptions
{
    public static string ConfigurationKey => "Keycloak";

    public required string AddressUri { get; set; }
    public required string HealthCheckAddressUri { get; set; }
    public required EndPoints EndPoints { get; set; }
    public Dictionary<string, Realm> Realms { get; set; } = [];

    // Helper methods to get specific realm clients
    public RealmClient GetBuiltInClient(string clientName = "AdminCli")
    {
        if (!Realms.TryGetValue("BuiltIn", out Realm? realm))
            throw new InvalidOperationException("BuiltIn realm not found in Keycloak configuration.");

        if (!realm.Clients.TryGetValue(clientName, out RealmClient? client))
            throw new InvalidOperationException($"{clientName} client not found in BuiltIn realm.");

        return client;
    }

    public RealmClient GetCustomClient(string clientName = "ArchetypeCli")
    {
        if (!Realms.TryGetValue("Custom", out Realm? realm))
            throw new InvalidOperationException("Custom realm not found in Keycloak configuration.");

        if (!realm.Clients.TryGetValue(clientName, out RealmClient? client))
            throw new InvalidOperationException($"{clientName} client not found in Custom realm.");

        return client;
    }

    public Realm GetBuiltInRealm()
    {
        if (!Realms.TryGetValue("BuiltIn", out Realm? realm))
            throw new InvalidOperationException("BuiltIn realm not found in Keycloak configuration.");

        return realm;
    }

    public Realm GetCustomRealm()
    {
        if (!Realms.TryGetValue("Custom", out Realm? realm))
            throw new InvalidOperationException("Custom realm not found in Keycloak configuration.");

        return realm;
    }
}

public class Realm
{
    public required string Name { get; set; }
    public Dictionary<string, RealmClient> Clients { get; set; } = [];
}

public class RealmClient
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string ClientId { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Scopes { get; set; }
    public string? Secret { get; set; }
}

public class EndPoints
{
    public required string Metadata { get; set; }
    public required string TokenExchange { get; set; }
    public required string CreateUser { get; set; }
    public required string LogoutUser { get; set; }
    public required string UserInfo { get; set; }
    public required string Introspect { get; set; }
    public required string Keys { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}