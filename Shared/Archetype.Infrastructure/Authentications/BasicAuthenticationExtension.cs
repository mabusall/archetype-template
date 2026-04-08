namespace Archetype.Infrastructure.Authentications;

public static class BasicAuthenticationExtension
{
    public static TBuilder AddBasicAuthentication<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder
            .Services
            .AddAuthorization()
            .AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

        return builder;
    }
}

public class BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                        ILoggerFactory logger,
                                        //AppSettingsKeyManagement appSettingsKeyManagement,
                                        UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out StringValues value))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization header missing"));
        }

        try
        {
            //var basicAuthenticationOptions = appSettingsKeyManagement.BasicAuthenticationOptions;
            BasicAuthenticationOptions basicAuthenticationOptions = new()
            {
                Password = "",
                UserName = ""
            };

            string authHeader = value.ToString();
            string authHeaderValue = authHeader.Split(' ')[1];
            string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderValue)).Split(':');

            string username = credentials[0];
            string password = credentials[1];

            if (username == basicAuthenticationOptions.UserName &&
                password == basicAuthenticationOptions.Password)
            {
                Claim[] claims = new[] { new Claim(ClaimTypes.Name, username) };
                ClaimsIdentity identity = new(claims, Scheme.Name);
                ClaimsPrincipal principal = new(identity);
                AuthenticationTicket ticket = new(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));
            }
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header format"));
        }
    }
}