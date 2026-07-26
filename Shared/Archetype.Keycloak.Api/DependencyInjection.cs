namespace Archetype.Keycloak.Api;

public static class DependencyInjection
{
    public static TBuilder AddKeycloakModule<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // Register MediatR handlers
        MemeoryStack.AddAssembly(Assembly.GetExecutingAssembly());


        builder
            .Services
            .AddHttpClient<KeycloakApiClient>(client =>
            {
                KeycloakOptions keycloakOptions = builder
                    .Configuration
                    .GetSection(KeycloakOptions.ConfigurationKey)
                    .Get<KeycloakOptions>()
                    ?? throw new InvalidOperationException($"Missing configuration section '{KeycloakOptions.ConfigurationKey}'."); ;

                string keycloak = keycloakOptions.AddressUri;

                // Ensure the base address ends with a slash
                if (!keycloak.EndsWith('/'))
                    keycloak = $"{keycloak}/";

                client.BaseAddress = new Uri(keycloak!);
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            })
            .AddResilienceHandler("keycloak-api", (pipeline, context) =>
            {
                // Get the logger from the context
                ILogger<KeycloakApiClient> logger = context.ServiceProvider.GetRequiredService<ILogger<KeycloakApiClient>>();

                // ✅ Retry (Polly v8)
                pipeline.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromMilliseconds(100),
                    BackoffType = DelayBackoffType.Exponential,

                    // Transient predicate provided by Microsoft.Extensions.Http.Resilience
                    ShouldHandle = args => ValueTask.FromResult(
                        HttpClientResiliencePredicates.IsTransient(args.Outcome))
                });


                // ✅ Circuit breaker (Polly v8)
                pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    // Small values for testing
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    FailureRatio = 0.5,        // 50% failures in the sampling window trips the breaker
                    MinimumThroughput = 2,     // minimum calls in sampling window to consider tripping
                    BreakDuration = TimeSpan.FromSeconds(15),

                    // 🟥 Circuit OPENED
                    OnOpened = args =>
                    {
                        if (logger.IsEnabled(LogLevel.Warning))
                        {
                            Outcome<HttpResponseMessage> lastFailure = args.Outcome;
                            string failureText = lastFailure.Exception?.Message
                                              ?? lastFailure.Result?.StatusCode.ToString()
                                              ?? "Unknown failure";

                            logger.LogWarning("Circuit OPENED for {BreakDuration}. Last failure: {Failure}",
                                              args.BreakDuration,
                                              failureText);
                        }
                        return ValueTask.CompletedTask;
                    },

                    //  Circuit HALF-OPEN
                    OnHalfOpened = args =>
                    {
                        if (logger.IsEnabled(LogLevel.Information))
                        {
                            logger.LogInformation("Circuit HALF-OPEN: probing service availability...");
                        }
                        return ValueTask.CompletedTask;
                    },

                    //  Circuit CLOSED
                    OnClosed = args =>
                    {
                        if (logger.IsEnabled(LogLevel.Information))
                        {
                            logger.LogInformation("Circuit CLOSED: service appears healthy again.");
                        }
                        return ValueTask.CompletedTask;
                    }
                });

                pipeline.AddTimeout(TimeSpan.FromSeconds(30));
            });

        return builder;
    }
}
