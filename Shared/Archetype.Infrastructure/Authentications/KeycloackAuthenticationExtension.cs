namespace Archetype.Infrastructure.Authentications;

public static class KeycloackAuthenticationExtension
{
    // C# 14 extension members style with generics declared on the extension block. [1](https://stackoverflow.com/questions/79825681/how-do-i-implement-c-sharp-14-extension-block-on-a-generic)[2](https://devblogs.microsoft.com/dotnet/csharp-exploring-extension-members/)
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        public TBuilder AddKeyclockAuthentication()
        {
            KeycloakOptions? keycloakOptions = builder.Configuration
                .GetSection(KeycloakOptions.ConfigurationKey)
                .Get<KeycloakOptions>();

            if (keycloakOptions is null)
                throw new InvalidOperationException($"Missing configuration section '{KeycloakOptions.ConfigurationKey}'.");

            if (keycloakOptions.EndPoints is null)
                throw new InvalidOperationException("Keycloak EndPoints configuration is missing.");

            string baseAddress = (keycloakOptions.AddressUri ?? string.Empty).TrimEnd('/');
            if (string.IsNullOrWhiteSpace(baseAddress))
                throw new InvalidOperationException("Keycloak AddressUri is missing or empty.");

            Realm customRealm = keycloakOptions.GetCustomRealm();
            RealmClient customClient = keycloakOptions.GetCustomClient("ArchetypeCli");

            if (string.IsNullOrWhiteSpace(customRealm.Name))
                throw new InvalidOperationException("Keycloak Custom Realm Name is missing or empty.");

            if (string.IsNullOrWhiteSpace(customClient.ClientId))
                throw new InvalidOperationException("Keycloak Custom ArchetypeCli Client ClientId is missing or empty.");

            // Build metadata address using your existing template:
            // "Metadata": "/realms/{0}/.well-known/openid-configuration"
            string metadataTemplate = keycloakOptions.EndPoints.Metadata ?? string.Empty;
            if (string.IsNullOrWhiteSpace(metadataTemplate))
                throw new InvalidOperationException("Keycloak EndPoints:Metadata is missing or empty.");

            string metaPath = string.Format(metadataTemplate, customRealm.Name);
            string metadataAddress = CombineUrl(baseAddress, metaPath);

            // Issuer expected in Keycloak tokens:
            string validIssuer = $"{baseAddress}/realms/{customRealm.Name}";

            // Derive HTTPS requirement from BaseAddress (no new config):
            bool requireHttps = baseAddress.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

            builder.Services
                .AddAuthorization()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    // Keep using MetadataAddress (your config already provides it).
                    options.MetadataAddress = metadataAddress;
                    options.RequireHttpsMetadata = requireHttps;
                    options.SaveToken = true;

                    // ✅ Enhancement: Use ConfigurationManager to fetch & cache OIDC configuration + JWKS signing keys.
                    // This avoids creating HttpClient per token and avoids sync-over-async GetResult().
                    options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                        metadataAddress,
                        new OpenIdConnectConfigurationRetriever(),
                        new HttpDocumentRetriever { RequireHttps = requireHttps });

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.FromMinutes(1),
                        ValidateLifetime = true,
                        ValidateAudience = false,

                        //ValidateAudience = true,
                        //ValidAudience = clientRealm.Client,

                        ValidateIssuer = true,
                        ValidIssuer = validIssuer,

                        ValidateIssuerSigningKey = true,

                        // ✅ Enhancement: signing keys resolved via OIDC configuration manager (cached).
                        IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                        {
                            // ConfigurationManager caches and refreshes automatically when needed.
                            OpenIdConnectConfiguration config = options.ConfigurationManager
                                .GetConfigurationAsync(default)
                                .GetAwaiter()
                                .GetResult();

                            return config.SigningKeys;
                        }
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            ClaimsIdentity? identity = context.Principal?.Identities.FirstOrDefault();
                            if (identity is null)
                                return Task.CompletedTask;

                            List<Claim> claimsToAdd = GetAllCustomClaims(identity.Claims, customClient);

                            if (claimsToAdd.Count > 0)
                                identity.AddClaims(claimsToAdd);

                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = MediaTypeNames.Application.Json;
                            context.NoResult();

                            return Task.CompletedTask;
                        }
                    };
                });

            return builder;
        }
    }

    private static string CombineUrl(string baseAddress, string relative)
    {
        baseAddress = (baseAddress ?? string.Empty).TrimEnd('/');
        relative = (relative ?? string.Empty).TrimStart('/');
        return $"{baseAddress}/{relative}";
    }

    private static List<Claim> GetAllCustomClaims(IEnumerable<Claim> allClaims, RealmClient client)
    {
        // Sonar-friendly: validate inputs explicitly
        ArgumentNullException.ThrowIfNull(allClaims);
        ArgumentNullException.ThrowIfNull(client);
        if (string.IsNullOrWhiteSpace(client.ClientId)) return [];

        string? resourceAccessJson = allClaims
            .FirstOrDefault(c => c.Type == "resource_access")?
            .Value;
        if (string.IsNullOrWhiteSpace(resourceAccessJson))
            return [];

        try
        {
            using JsonDocument doc = JsonDocument.Parse(resourceAccessJson);

            if (!doc.RootElement.TryGetProperty(client.ClientId, out JsonElement clientNode))
                return [];

            if (!clientNode.TryGetProperty("roles", out JsonElement rolesNode) ||
                rolesNode.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            // Pre-size list to reduce resizing (Sonar often likes this sort of micro-optimization)
            List<Claim> claims = new(rolesNode.GetArrayLength());

            foreach (JsonElement roleElement in rolesNode.EnumerateArray())
            {
                if (roleElement.ValueKind != JsonValueKind.String)
                    continue;

                string? role = roleElement.GetString();
                if (string.IsNullOrWhiteSpace(role))
                    continue;

                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
        catch (JsonException)
        {
            // Intentionally ignored: malformed Keycloak claim shouldn't fail authentication,
            // only skip role enrichment.
            return [];
        }
    }
}