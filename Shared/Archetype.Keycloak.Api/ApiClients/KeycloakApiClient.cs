namespace Archetype.Keycloak.Api.ApiClients;

internal class KeycloakApiClient
{
    private readonly KeycloakAdminClient _admin;

    public KeycloakAdminClient Admin => _admin;
    public KeycloakRealmClient Realm { get; }

    public KeycloakApiClient(HttpClient httpClient,
                             AppSettingsKeyManagement appSettingsKeyManagement)
    {
        _admin = new KeycloakAdminClient(httpClient, appSettingsKeyManagement);
        Realm = new KeycloakRealmClient(httpClient, _admin, appSettingsKeyManagement);
    }
}

#region [ keycloak admin ]

public interface IKeycloakAdminClient
{
    Task<IList<KeycloakAdminClientsResult>> GetAllClientsAsync(CancellationToken cancellationToken);
    Task<KeycloakAdminLoginResult> LoginAsync(CancellationToken cancellationToken);
    Task<KeycloakSuccessStatusResult> LogoutAsync(string refreshToken, CancellationToken cancellationToken);
}

public class KeycloakAdminClient(HttpClient httpClient,
                                 AppSettingsKeyManagement appSettingsKeyManagement)
    : IKeycloakAdminClient
{
    public async Task<IList<KeycloakAdminClientsResult>> GetAllClientsAsync(CancellationToken cancellationToken)
    {
        // get keycloak admin token
        KeycloakAdminLoginResult login = await LoginAsync(cancellationToken)
            ?? throw new InvalidOperationException(ErrorMessageResources.InvalidOperation);
        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm adminRealm = options!.GetBuiltInRealm();
        string url = $"admin/realms/{adminRealm.Name}/clients";
        using HttpRequestMessage httpRequest = new(HttpMethod.Get, url);
        httpClient.AttachBearerAuthentication(login.AccessToken!);

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // logout admin session
        await LogoutAsync(login.RefreshToken!, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        // cast the result
        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        // Fix: Ensure non-null return value
        return json.Deserialize<IList<KeycloakAdminClientsResult>>()
             ?? throw new InvalidOperationException(ErrorMessageResources.SerializationFailed);
    }

    public async Task<KeycloakAdminLoginResult> LoginAsync(CancellationToken cancellationToken)
    {
        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm adminRealm = options!.GetBuiltInRealm();
        RealmClient adminClient = options.GetBuiltInClient("AdminCli");
        string url = string.Format(options.EndPoints.TokenExchange, adminRealm.Name);
        using FormUrlEncodedContent content = new(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = adminClient.ClientId,
            ["username"] = adminClient.UserName!,
            ["password"] = adminClient.Password!,
        });
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = content
        };

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        // cast the result
        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        KeycloakAdminLoginResult result = json.Deserialize<KeycloakAdminLoginResult>()
            ?? throw new InvalidOperationException(ErrorMessageResources.SerializationFailed);

        return result;
    }

    public async Task<KeycloakSuccessStatusResult> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm adminRealm = options!.GetBuiltInRealm();
        RealmClient adminClient = options.GetBuiltInClient("AdminCli");
        string url = string.Format(options.EndPoints.LogoutUser, adminRealm.Name);
        using FormUrlEncodedContent content = new(new Dictionary<string, string>
        {
            ["client_id"] = adminClient.ClientId,
            ["refresh_token"] = refreshToken,
        });
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = content
        };

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        return new KeycloakSuccessStatusResult(true);
    }
}

#endregion

#region [ keycloak custom realm ]

public interface IKeycloakRealmClient
{
    Task<KeycloakSuccessStatusResult> CreateUserAsync(KeycloakRealmCreateUserCommand command, CancellationToken cancellationToken);
    Task<KeycloakSuccessStatusResult> DeleteUserAsync(KeycloakRealmDeleteUserCommand command, CancellationToken cancellationToken);
    Task<KeycloakSuccessStatusResult> ExtendUserSessionAsync(KeycloakRealmExtendUserSessionCommand command, CancellationToken cancellationToken);
    Task<IList<KeycloakRealmGetRealmsResult>> GetAllClientsAsync(CancellationToken cancellationToken);
    Task<IList<KeycloakRealmGetUserResult>> GetUserAsync(KeycloakRealmGetUserQuery query, CancellationToken cancellationToken);
    Task<KeycloakRealmGetUserIntrospectResult> GetUserIntrospectAsync(KeycloakRealmGetUserIntrospectQuery query, CancellationToken cancellationToken);
    Task<IList<KeycloakRealmGetUserSessionsResult>> GetUserSessionsAsync(KeycloakRealmGetUserSessionsQuery query, CancellationToken cancellationToken);
    Task<KeycloakSuccessStatusResult> KillUserSessionAsync(KeycloakRealmKillUserSessionCommand command, CancellationToken cancellationToken);
    Task<KeycloakRealmLoginResult> Login(KeycloakRealmLoginCommand command, CancellationToken cancellationToken);
    Task<KeycloakSuccessStatusResult> Logout(KeycloakRealmLogoutCommand command, CancellationToken cancellationToken);
    Task<KeycloakRealmRefreshTokenResult> RefreshToken(KeycloakRealmRefreshTokenCommand command, CancellationToken cancellationToken);
    Task<KeycloakSuccessStatusResult> ResetPassword(KeycloakRealmResetPasswordCommand command, CancellationToken cancellationToken);
}

public class KeycloakRealmClient(HttpClient httpClient,
                                 KeycloakAdminClient adminClient,
                                 AppSettingsKeyManagement appSettingsKeyManagement)
    : IKeycloakRealmClient
{
    public async Task<KeycloakSuccessStatusResult> CreateUserAsync(KeycloakRealmCreateUserCommand command, CancellationToken cancellationToken)
    {
        // get keycloak admin token
        KeycloakAdminLoginResult login = await adminClient.LoginAsync(cancellationToken);

        // prepare create user body
        var createUserBody = new
        {
            username = command.UserName,
            email = command.Email,
            enabled = command.IsActive,
            firstName = command.FirstName,
            lastName = command.LastName,
            emailVerified = true,
        };

        // create body request
        string body = await JsonContent
            .Create(createUserBody)
            .ReadAsStringAsync(cancellationToken);

        // create user process
        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        string url = string.Format(options.EndPoints.CreateUser, clientRealm.Name);
        using StringContent content = new(body, Encoding.UTF8, MediaTypeNames.Application.Json);
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = content
        };

        // attach bearer token
        httpClient.AttachBearerAuthentication(login.AccessToken!);

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // logout admin session
        await adminClient.LogoutAsync(login.RefreshToken!, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        //... success
        return new KeycloakSuccessStatusResult(true);
    }

    public async Task<KeycloakSuccessStatusResult> DeleteUserAsync(KeycloakRealmDeleteUserCommand command, CancellationToken cancellationToken)
    {
        // get keycloak admin token
        KeycloakAdminLoginResult login = await adminClient.LoginAsync(cancellationToken);

        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        string url = $"admin/realms/{clientRealm.Name}/users/{command.Id}";
        using HttpRequestMessage httpRequest = new(HttpMethod.Delete, url);
        httpClient.AttachBearerAuthentication(login.AccessToken!);

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // logout admin session
        await adminClient.LogoutAsync(login.RefreshToken!, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        return new KeycloakSuccessStatusResult(true);
    }

    public async Task<KeycloakSuccessStatusResult> ExtendUserSessionAsync(KeycloakRealmExtendUserSessionCommand command, CancellationToken cancellationToken)
    {
        KeycloakAdminLoginResult login = await adminClient.LoginAsync(cancellationToken);

        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        RealmClient customClient = options.GetCustomClient("TasheerCli");
        string url = string.Format(options.EndPoints.TokenExchange, clientRealm.Name);
        using FormUrlEncodedContent content = new(new Dictionary<string, string>
        {
            ["client_id"] = customClient.ClientId,
            ["client_secret"] = customClient.Secret!,
            ["refresh_token"] = command.RefreshToken,
            ["grant_type"] = "refresh_token",
        });
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = content
        };

        // attach bearer token
        httpClient.AttachBearerAuthentication(login.AccessToken!);

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // logout admin session
        await adminClient.LogoutAsync(login.RefreshToken!, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        return new KeycloakSuccessStatusResult(true);
    }

    public async Task<IList<KeycloakRealmGetRealmsResult>> GetAllClientsAsync(CancellationToken cancellationToken)
    {
        // get keycloak admin token
        KeycloakAdminLoginResult login = await adminClient.LoginAsync(cancellationToken);
        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        string url = $"admin/realms/{clientRealm.Name}/clients";
        using HttpRequestMessage httpRequest = new(HttpMethod.Get, url);
        httpClient.AttachBearerAuthentication(login.AccessToken!);

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // logout admin session
        await adminClient.LogoutAsync(login.RefreshToken!, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        // cast the result
        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        return json.Deserialize<IList<KeycloakRealmGetRealmsResult>>()
            ?? throw new InvalidOperationException(ErrorMessageResources.SerializationFailed);
    }

    public async Task<IList<KeycloakRealmGetUserResult>> GetUserAsync(KeycloakRealmGetUserQuery query, CancellationToken cancellationToken)
    {
        // get keycloak admin token
        KeycloakAdminLoginResult login = await adminClient.LoginAsync(cancellationToken);

        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        string url = $"admin/realms/{clientRealm.Name}/users?exact=true&email={query.Email}";
        using HttpRequestMessage httpRequest = new(HttpMethod.Get, url);
        httpClient.AttachBearerAuthentication(login.AccessToken!);

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // logout admin session
        await adminClient.LogoutAsync(login.RefreshToken!, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        // cast the result
        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        return json.Deserialize<IList<KeycloakRealmGetUserResult>>()
            ?? throw new InvalidOperationException(ErrorMessageResources.SerializationFailed);
    }

    public async Task<KeycloakRealmGetUserIntrospectResult> GetUserIntrospectAsync(KeycloakRealmGetUserIntrospectQuery query, CancellationToken cancellationToken)
    {
        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        RealmClient customClient = options.GetCustomClient("TasheerCli");
        string url = string.Format(options!.EndPoints.Introspect, clientRealm.Name);
        using FormUrlEncodedContent content = new(new Dictionary<string, string>
        {
            ["token"] = query.AccessToken,
            ["client_id"] = customClient.ClientId,
            ["client_secret"] = customClient.Secret!,
        });
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = content
        };

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // cast the result
        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        return json.Deserialize<KeycloakRealmGetUserIntrospectResult>()
            ?? throw new InvalidOperationException(ErrorMessageResources.SerializationFailed);
    }

    public async Task<IList<KeycloakRealmGetUserSessionsResult>> GetUserSessionsAsync(KeycloakRealmGetUserSessionsQuery query, CancellationToken cancellationToken)
    {
        KeycloakRealmGetUserResult userInfo = (await GetUserAsync(new KeycloakRealmGetUserQuery(query.Email), cancellationToken))
            .FirstOrDefault()
            ?? throw new BadRequestException("User not found");

        // get keycloak admin token
        KeycloakAdminLoginResult login = await adminClient.LoginAsync(cancellationToken);

        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        string url = $"admin/realms/{clientRealm.Name}/users/{userInfo.Id}/sessions";
        using HttpRequestMessage httpRequest = new(HttpMethod.Get, url);

        // attach bearer token
        httpClient.AttachBearerAuthentication(login.AccessToken!);

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // logout admin session
        await adminClient.LogoutAsync(login.RefreshToken!, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        // cast the result
        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        return json.Deserialize<IList<KeycloakRealmGetUserSessionsResult>>()
            ?? throw new InvalidOperationException(ErrorMessageResources.SerializationFailed);
    }

    public async Task<KeycloakSuccessStatusResult> KillUserSessionAsync(KeycloakRealmKillUserSessionCommand command, CancellationToken cancellationToken)
    {
        // get keycloak admin token
        KeycloakAdminLoginResult login = await adminClient.LoginAsync(cancellationToken);

        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        string url = $"admin/realms/{clientRealm.Name}/sessions/{command.SessionId}";
        using HttpRequestMessage httpRequest = new(HttpMethod.Delete, url);
        httpClient.AttachBearerAuthentication(login.AccessToken!);

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // logout admin session
        await adminClient.LogoutAsync(login.RefreshToken!, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        return new KeycloakSuccessStatusResult(true);
    }

    public async Task<KeycloakRealmLoginResult> Login(KeycloakRealmLoginCommand command, CancellationToken cancellationToken)
    {
        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        RealmClient customClient = options.GetCustomClient("TasheerCli");
        string url = string.Format($"{options.EndPoints.TokenExchange}", clientRealm.Name);
        using FormUrlEncodedContent content = new(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = customClient.ClientId,
            ["client_secret"] = customClient.Secret!,
            ["username"] = command.Email,
            ["password"] = command.Password,
            ["scope"] = customClient.Scopes!
        });
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = content
        };

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        // cast the result
        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        return json.Deserialize<KeycloakRealmLoginResult>()
             ?? throw new InvalidOperationException(ErrorMessageResources.SerializationFailed);
    }

    public async Task<KeycloakSuccessStatusResult> Logout(KeycloakRealmLogoutCommand command, CancellationToken cancellationToken)
    {
        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        RealmClient customClient = options.GetCustomClient("TasheerCli");
        string url = string.Format(options.EndPoints.LogoutUser, clientRealm.Name);
        using FormUrlEncodedContent content = new(new Dictionary<string, string>
        {
            ["client_id"] = customClient.ClientId,
            ["client_secret"] = customClient.Secret!,
            ["refresh_token"] = command.RefreshToken,
        });
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = content
        };

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        return new KeycloakSuccessStatusResult(true);
    }

    public async Task<KeycloakRealmRefreshTokenResult> RefreshToken(KeycloakRealmRefreshTokenCommand command, CancellationToken cancellationToken)
    {
        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        RealmClient customClient = options.GetCustomClient("TasheerCli");
        string url = string.Format(options.EndPoints.TokenExchange, clientRealm.Name);
        using FormUrlEncodedContent content = new(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = command.RefreshToken,
            ["client_id"] = customClient.ClientId,
            ["client_secret"] = customClient.Secret!,
        });

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
        {
            Content = content
        };

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // validate the response
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
            throw new UnauthorizedException("Invalid refresh token");
        }

        // validate the response
        response.EnsureSuccessStatusCode();

        // cast the result
        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        return json.Deserialize<KeycloakRealmRefreshTokenResult>()
            ?? throw new InvalidOperationException(ErrorMessageResources.SerializationFailed);
    }

    public async Task<KeycloakSuccessStatusResult> ResetPassword(KeycloakRealmResetPasswordCommand command, CancellationToken cancellationToken)
    {
        KeycloakRealmGetUserResult userInfo = (await GetUserAsync(new KeycloakRealmGetUserQuery(command.Email), cancellationToken))
            .FirstOrDefault()
            ??
            throw new BadRequestException(ErrorMessageResources.Keycloak_EmailNotFound);

        // get keycloak admin token
        KeycloakAdminLoginResult login = await adminClient.LoginAsync(cancellationToken);

        var resetPasswordBody = new
        {
            type = "password",
            temporary = false,
            value = command.Password,
        };

        // create body request
        string body = await JsonContent
            .Create(resetPasswordBody)
            .ReadAsStringAsync(cancellationToken);

        KeycloakOptions options = appSettingsKeyManagement.KeycloakOptions;
        Realm clientRealm = options!.GetCustomRealm();
        string url = string.Format($"{options.EndPoints.CreateUser}/{userInfo.Id}/reset-password",
            clientRealm.Name);
        using StringContent content = new(body, Encoding.UTF8, MediaTypeNames.Application.Json);

        using HttpRequestMessage httpRequest = new(HttpMethod.Put, url)
        {
            Content = content
        };
        httpClient.AttachBearerAuthentication(login.AccessToken!);

        // send the request
        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        // logout admin session
        await adminClient.LogoutAsync(login.RefreshToken!, cancellationToken);

        // validate the response
        response.EnsureSuccessStatusCode();

        return new KeycloakSuccessStatusResult(true);
    }
}

#endregion