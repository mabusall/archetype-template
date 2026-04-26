namespace Sample.Core.Api;

static class DependencyInjection
{
    public static TBuilder AddSampleCoreModule<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        ApplicationOptions applicationOptions = builder
            .Configuration
            .GetSection(ApplicationOptions.ConfigurationKey)
            .Get<ApplicationOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{ApplicationOptions.ConfigurationKey}'.");

        // Register MediatR handlers
        MemeoryStack.AddAssembly(Assembly.GetExecutingAssembly());

        builder
            .AddNotificationModule()
            .AddKeycloakModule();

        // add other services
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

    public static WebApplication UseSampleCoreModule(this WebApplication app)
    {
        app
            .UseMigration<SampleDbContext>()
            .MapOrderEndpoints();

        return app;
    }
}
