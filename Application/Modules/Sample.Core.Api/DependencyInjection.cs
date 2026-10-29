namespace Sample.Core.Api;

static class DependencyInjection
{
    public static TBuilder AddSampleCoreModule<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // Register MediatR handlers
        MemeoryStack.AddAssembly(Assembly.GetExecutingAssembly());

        builder
            .AddNotificationModule()
            .AddKeycloakModule()
            .AddDatabase()
            .AddCountriesNowApi();

        return builder;
    }

    internal static TBuilder AddDatabase<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        ApplicationOptions applicationOptions = builder
            .Configuration
            .GetSection(ApplicationOptions.ConfigurationKey)
            .Get<ApplicationOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{ApplicationOptions.ConfigurationKey}'.");

        // add database services
        builder
            .Services

            // add unit of work
            .AddScoped<ISampleDbUnitOfWork, SampleDbUnitOfWork>()

            // add db context
            .AddDbContext<SampleDbContext>((serviceProvider, options) =>
            {
                options.UseConfiguredProvider(applicationOptions);

                if (builder.Environment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }
            });

        return builder;
    }

    internal static TBuilder AddCountriesNowApi<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        IntegrationApisOptions integrationApisOptions = builder
            .Configuration
            .GetSection(IntegrationApisOptions.ConfigurationKey)
            .Get<IntegrationApisOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{IntegrationApisOptions.ConfigurationKey}'.");

        IntegrationApi countriesNowApiOptions = integrationApisOptions.Apis["CountriesNow"];

        builder
            .Services
            .AddHttpClient<CountriesNowApiClient>(client =>
            {
                client.BaseAddress = new Uri(countriesNowApiOptions.AddressUri!);
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json));
            })
            .AddResilienceHandler("countriesnow-api", (pipeline, context) =>
            {
                // Get the logger from the context
                ILogger<CountriesNowApiClient> logger = context.ServiceProvider.GetRequiredService<ILogger<CountriesNowApiClient>>();

                // ✅ Retry (Polly v8)
                pipeline.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromMilliseconds(100),
                    BackoffType = DelayBackoffType.Exponential,

                    // Transient predicate provided by Microsoft.Extensions.Http.Resilience
                    ShouldHandle = args => ValueTask.FromResult(
                        HttpClientResiliencePredicates.IsTransient(args.Outcome))
                }).AddStandardCircuitBreaker(logger, "CountriesNow");

                pipeline.AddTimeout(TimeSpan.FromMinutes(3));
            });

        return builder;
    }

    public static WebApplication UseSampleCoreModule(this WebApplication app)
    {
        app
            .UseMigration<SampleDbContext>()
            .MapOrderEndpoints();

        return app;
    }
}
