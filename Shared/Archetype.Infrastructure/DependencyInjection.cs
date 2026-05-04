namespace Archetype.Infrastructure;

public static class DependencyInjection
{
    private const string ReadyTag = "ready";

    public static TBuilder AddInfrastructureServices<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        ApplicationOptions applicationOptions = builder
            .Configuration
            .GetSection(ApplicationOptions.ConfigurationKey)
            .Get<ApplicationOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{ApplicationOptions.ConfigurationKey}'.");

        // Configure settings and Data Protection
        ApiKeyFilter.ApiKey = applicationOptions.ApiKey;

        // load required keys in order to use it throwgh all application life cycle
        DataProtectionProviderExtention.Initialize(applicationOptions.PublicKey,
                                                   applicationOptions.PrivateKey);

        // Configure logging FIRST (before anything else)
        builder.ConfigureLogging();

        // Configure observability (OpenTelemetry)
        builder.ConfigureObservability();

        // Configure common services and defaults
        builder.ConfigureServiceDefaults();

        // Configure Redis if enabled
        builder.ConfigureRedis();

        // Configure CORS
        builder.ConfigureCors();

        // Keycloak Authentication
        builder.AddKeyclockAuthentication();

        // Add Hangfire background jobs
        builder.ConfigureHangfire();

        // Add message queue services (MassTransit + RabbitMQ + Outbox)
        builder.ConfigureMessageQueue();

        // Configure comprehensive health checks
        builder.ConfigureAdvancedHealthChecks();

        // When mapping objects with circular references,
        // a stackoverflow exception will result.
        // This is because Mapster will get stuck in a loop
        // trying to recursively map the circular reference.
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

        return builder;
    }

    public static WebApplication UseInfrastructureServices(this WebApplication app, bool initApiServices)
    {
        ArgumentNullException.ThrowIfNull(app);

        HangfireOptions hangfireOptions = app
            .Configuration
            .GetSection(HangfireOptions.ConfigurationKey)
            .Get<HangfireOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{HangfireOptions.ConfigurationKey}'.");

        ApplicationOptions applicationOptions = app
            .Configuration
            .GetSection(ApplicationOptions.ConfigurationKey)
            .Get<ApplicationOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{ApplicationOptions.ConfigurationKey}'.");

        CorsConfigurationOptions corsOptions = app
            .Configuration
            .GetSection(CorsConfigurationOptions.ConfigurationKey)
            .Get<CorsConfigurationOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{CorsConfigurationOptions.ConfigurationKey}'.");

        List<CultureInfo> supportedCultures = applicationOptions
            .SupportedLanguages
            .Select(language => new CultureInfo(language))
            .ToList();

        RequestLocalizationOptions localizationOptions = new()
        {
            DefaultRequestCulture = new RequestCulture("en"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        };

        // Enable response compression for API endpoints if configured (improves performance)
        if (initApiServices)
            app.UseResponseCompression();

        app
            // 1. Apply any pending migrations (before anything else)
            .UseMigration<OutBoxDbContext>()

            // 2. HTTPS redirection (as early as possible)
            .UseHttpsRedirection()

            // 3. Correlation ID (for logging/tracing throughout the pipeline)
            // some fixes by Mohammed Al Turaif
            .UseCorrelationId()

            // 4. Global exception handler (MUST be early to catch all exceptions)
            .UseExceptionHandler(options => { })

            // 5. Static files (can short-circuit early, no auth needed)
            .UseStaticFiles()

            // 6. CORS (BEFORE authentication/authorization)
            .UseCorsIfEnabled(corsOptions)

            // 7. Rate limiting (protect your API early)
            .UseRateLimiter()

            // 8. Localization (set culture for the request)
            .UseRequestLocalization(localizationOptions)

            // 9. Custom localization middleware (depends on culture being set)
            .UseMiddleware<LocalizationMiddleware>()

            // 10. Output caching (BEFORE authentication for public endpoints)
            .UseOutputCache()

            // 11. Authentication (identify the user)
            .UseAuthentication()

            // 12. Authorization (check permissions)
            .UseAuthorization();

        if (hangfireOptions.IsEnabled)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = [new HangfireDashboardAllowAllAuthorizationFilter()],
                DashboardTitle = "Archetype Jobs Dashboard"
            });

            foreach (HangfireJob? job in hangfireOptions.Tasks.Select(s => s.Value))
            {
                // Register each Hangfire job dynamically based on configuration
                if (job.IsEnabled)
                {
                    Type jobType = Type.GetType(job.Handler)
                        ?? throw new InvalidOperationException($"Unable to resolve job type '{job.Handler}'.");
                    string jobKey = job.Handler.ToEndpointName(hangfireOptions.Prefix);
                    string cron = job.Schedule.ToCronExpression();

                    // Register the job with CancellationToken support
                    TaskManager.RegisterRecurringJob(jobKey, jobType, cron);
                }
            }
        }

        // Map health check endpoints
        app.MapHealthChecks();

        // Map ping endpoint for quick liveness checks
        app.MapPing();

        // flush and close Serilog on application shutdown
        IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(() =>
        {
            Log.Information("Application is shutting down...");
            Log.CloseAndFlush();
        });

        return app;
    }

    public static TBuilder AddApiServiceDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // Add environment variables with specific prefix
        // Add HashiCorp Vault configuration source
        builder
            .Configuration
            .AddEnvironmentVariables("ArchetypeKeys:")
            .AddHashiCorpVaultAsync(VaultSettingOptions.ConfigurationKey)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        // Configure API Versioning
        builder
            .Services
            // Add services to the container.
            .AddOpenApi()

            .AddResponseCompression(options =>
            {
                // Enable compression for HTTPS (secure connections)
                options.EnableForHttps = true;

                // Add Gzip compression (widely supported)
                options.Providers.Add<GzipCompressionProvider>();

                // Add Brotli compression (better compression, modern browsers)
                options.Providers.Add<BrotliCompressionProvider>();

                // Compress these MIME types
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                [
                    MediaTypeNames.Application.Json,
                    MediaTypeNames.Application.Xml,
                    MediaTypeNames.Text.Plain,
                    MediaTypeNames.Text.Html
                ]);
            })

            // Configure compression levels for better performance
            .Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest; // Balance between speed and size
            })

            .Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest; // Balance between speed and size
            })

            // Add API versioning to support multiple versions of your API simultaneously
            .AddApiVersioning(options =>
            {
                // Default version if not specified
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);

                // Assume v1.0 if client doesn't specify version
                options.AssumeDefaultVersionWhenUnspecified = true;

                // Read version from URL segment (e.g., /v1/order, /v2/order)
                options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();

                // Report API versions in response headers (X-Api-Version, api-supported-versions)
                options.ReportApiVersions = true;
            });

        return builder;
    }

    public static TBuilder AddInMemoryTokenRing<TBuilder>(this TBuilder builder, Action<TokenRingOptions> configureOptions)
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configureOptions);

        TokenRingOptions options = new();
        configureOptions(options);

        if (string.IsNullOrWhiteSpace(options.CacheKey))
            throw new ArgumentException("CacheKey is required for TokenRing.", nameof(configureOptions));

        if (options.Tokens is null || options.Tokens.Count == 0)
            throw new ArgumentException("At least one token must be provided for TokenRing.", nameof(configureOptions));

        builder.Services.AddSingleton<ITokenRing>(sp =>
        {
            IDistributedCache distributedCache = sp.GetRequiredService<IDistributedCache>();
            return new TokenRing(distributedCache, options.CacheKey, options.Tokens);
        });

        return builder;
    }

    #region [ private methods ]

    private static TBuilder ConfigureServiceDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        IReadOnlyCollection<Assembly> assemblies = MemeoryStack.GetAll();

        builder
            .Services
            .AddHttpContextAccessor()
            .AddSingleton<IIdGeneratorService, IdGeneratorService>()
            .AddSingleton(_ => ConfigureAppSettings(builder.Configuration))
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<ICorrelationIdService, CorrelationIdService>()
            .AddExceptionHandler<CustomExceptionHandler>()

            // Add FluentValidation
            .AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true)

            // Add custom Archetype Mediator with behaviors
            .AddArchetypeMediator(assemblies, config =>
            {
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            })

            // Configure rate limiting
            .AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("10RequestsPerMinute", config =>
                {
                    config.PermitLimit = 10;
                    config.Window = TimeSpan.FromMinutes(1);
                    config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    config.QueueLimit = 0;
                });

                options.OnRejected = (context, token) =>
                {
                    throw new RateLimitExceededException("Too many requests. Try again later.");
                };
            })

            // configure default output caching
            .AddOutputCache(options =>
            {
                // Define a global caching policy (optional)
                options.AddPolicy("Default30Seconds", policy =>
                    policy.Expire(TimeSpan.FromSeconds(30)));

                // Another policy for specific routes
                options.AddPolicy("Default1Minute", policy =>
                    policy.Expire(TimeSpan.FromMinutes(1)));
            })

            // Configure JSON options for minimal APIs.
            .Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            })

            .Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

        return builder;
    }

    private static TBuilder ConfigureHangfire<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        HangfireOptions hangfireOptions = builder
            .Configuration
            .GetSection(HangfireOptions.ConfigurationKey)
            .Get<HangfireOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{HangfireOptions.ConfigurationKey}'.");

        ApplicationOptions applicationOptions = builder
            .Configuration
            .GetSection(ApplicationOptions.ConfigurationKey)
            .Get<ApplicationOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{ApplicationOptions.ConfigurationKey}'.");

        // If Hangfire is not enabled, skip the configuration
        if (!hangfireOptions.IsEnabled)
            return builder;

        ArgumentException.ThrowIfNullOrWhiteSpace(applicationOptions.DbConnection);

        // Register HangfireLoggingFilter in DI for dependency resolution
        builder.Services.AddSingleton<HangfireLoggingFilter>();

        builder
            .Services
            .AddHangfire((serviceProvider, config) => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()

                .UseFilter(new AutomaticRetryAttribute
                {
                    Attempts = 0,
                    OnAttemptsExceeded = AttemptsExceededAction.Fail
                })

                // Add global logging filter (similar to MediatR LoggingBehavior and MassTransit LoggingConsumeFilter)
                // Automatically logs all job activity with correlation IDs, timing, and error tracking
                .UseFilter(serviceProvider.GetRequiredService<HangfireLoggingFilter>())

                .UseSerializerSettings(new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                })
                .UseConfiguredHangfireStorage(applicationOptions, hangfireOptions))
            .AddHangfireServer();

        return builder;
    }

    private static TBuilder ConfigureRedis<TBuilder>(this TBuilder builder)
       where TBuilder : IHostApplicationBuilder
    {
        RedisOptions redisOptions = builder
            .Configuration
            .GetSection(RedisOptions.ConfigurationKey)
            .Get<RedisOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{RedisOptions.ConfigurationKey}'.");

        // If Redis is not enabled, skip the configuration
        if (!redisOptions.IsEnabled)
            return builder;

        ArgumentException.ThrowIfNullOrWhiteSpace(redisOptions.AddressUri);

        string connectionString = $"{redisOptions.AddressUri},abortConnect=False";

        // Add SSL option
        if (redisOptions.SecureSsl)
            connectionString = $"{connectionString},ssl=True";
        else
            connectionString = $"{connectionString},ssl=False";

        // Add password if provided
        if (!string.IsNullOrWhiteSpace(redisOptions.Password))
            connectionString = $"{connectionString},password={redisOptions.Password}";

        // Add Redis Output Caching (for OutputCache middleware)
        builder
            .Services
            .AddStackExchangeRedisOutputCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = redisOptions.OutputCachePrefix;
            });

        // Add Redis Distributed Cache (for IDistributedCache)
        builder
            .Services
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = redisOptions.DistributedCachePrefix;
            });

        return builder;
    }

    private static TBuilder ConfigureCors<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        CorsConfigurationOptions? corsConfigurationOptions = builder
            .Configuration
            .GetSection(CorsConfigurationOptions.ConfigurationKey)
            .Get<CorsConfigurationOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{CorsConfigurationOptions.ConfigurationKey}'.");

        // If CORS is not configured or disabled, skip
        if (corsConfigurationOptions is null || !corsConfigurationOptions.IsEnabled)
            return builder;

        ArgumentException.ThrowIfNullOrWhiteSpace(corsConfigurationOptions.PolicyName);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsConfigurationOptions.PolicyName, policy =>
            {
                // WARNING: AllowAnyOrigin is insecure - only for development!
                if (corsConfigurationOptions.AllowAnyOrigin)
                {
                    policy.AllowAnyOrigin();

                    // Note: Cannot use AllowCredentials() with AllowAnyOrigin()
                    // This is a security restriction by design
                }
                else if (corsConfigurationOptions.AllowedOrigins.Length > 0)
                {
                    policy.WithOrigins(corsConfigurationOptions.AllowedOrigins);

                    if (corsConfigurationOptions.AllowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "CORS is enabled but no origins configured. " +
                        "Set 'Cors:AllowedOrigins' with specific URLs or set 'Cors:AllowAnyOrigin' to true (dev only).");
                }

                // Configure headers
                if (corsConfigurationOptions.AllowedHeaders.Length > 0)
                {
                    policy.WithHeaders(corsConfigurationOptions.AllowedHeaders);
                }
                else
                {
                    policy.AllowAnyHeader();
                }

                // Configure methods
                if (corsConfigurationOptions.AllowedMethods.Length > 0)
                {
                    policy.WithMethods(corsConfigurationOptions.AllowedMethods);
                }
                else
                {
                    policy.AllowAnyMethod();
                }

                // Expose custom headers to client
                if (corsConfigurationOptions.ExposedHeaders.Length > 0)
                {
                    policy.WithExposedHeaders(corsConfigurationOptions.ExposedHeaders);
                }

                // Set preflight cache duration
                policy.SetPreflightMaxAge(TimeSpan.FromSeconds(corsConfigurationOptions.PreflightMaxAgeSeconds));
            });
        });

        return builder;
    }

    private static TBuilder ConfigureMessageQueue<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        RabbitMqOptions rabbitMqOptions = builder
            .Configuration
            .GetSection(RabbitMqOptions.ConfigurationKey)
            .Get<RabbitMqOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{RabbitMqOptions.ConfigurationKey}'.");

        ApplicationOptions applicationOptions = builder
            .Configuration
            .GetSection(ApplicationOptions.ConfigurationKey)
            .Get<ApplicationOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{ApplicationOptions.ConfigurationKey}'.");

        // If RabbitMQ is not active, skip the configuration
        if (!rabbitMqOptions.IsEnabled)
            return builder;

        // Outbox DbContext + bus helper service
        builder
            .Services
            .AddScoped<IBusService, BusService>()
            .AddDbContext<OutBoxDbContext>((_, options) =>
            {
                options.UseConfiguredProvider(applicationOptions);

                if (builder.Environment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }
            })

            // Add health checks for RabbitMQ and SQL Server (Outbox DB)
            .AddHealthChecks();

        foreach (ConsumerEndpoint endpoint in rabbitMqOptions.Consumers.Values)
        {
            if (endpoint!.IsActive)
            {
                // Get the consumer type using reflection
                Type? consumerType = Type.GetType(endpoint.Consumer, throwOnError: false);

                // If the type is not found, skip to the next endpoint
                if (consumerType is null)
                    continue;

                BusConfiguration.AddBusConfigurator(busConfigurator =>
                {
                    busConfigurator.AddConsumer(consumerType);
                });

                BusConfiguration.AddBusFactoryConfigurator((busFactoryConfigurator, registration) =>
                {
                    // Generate endpoint name using the consumer type and prefix
                    string endpointName = endpoint
                        .Consumer
                        .ToEndpointName(rabbitMqOptions.Prefix);

                    busFactoryConfigurator.ReceiveEndpoint(endpointName,
                        e =>
                        {
                            e.PrefetchCount = endpoint.PrefetchCount;
                            e.ConcurrentMessageLimit = endpoint.ConcurrentMessageLimit;
                            e.UseMessageRetry(r => r.Interval(endpoint.RetryCount, endpoint.Interval));
                            if (endpoint.UseDatabase)
                            {
                                e.UseEntityFrameworkOutbox<OutBoxDbContext>(registration);
                            }

                            // finally configure the consumer as per endpoint configuration
                            e.ConfigureConsumer(registration, consumerType);
                        });
                });
            }
        }

        builder
            .Services
            .AddMassTransit(options =>
            {
                options.SetKebabCaseEndpointNameFormatter();
                options.SetInMemorySagaRepositoryProvider();

                options.AddEntityFrameworkOutbox<OutBoxDbContext>(efConfig =>
                {
                    // configure which database lock provider to use based on appsettings
                    efConfig.ConfigureMassTransitOutbox(applicationOptions);

                    // enable the bus outbox based on configuration
                    efConfig.UseBusOutbox(b =>
                    {
                        if (rabbitMqOptions.DisableDeliveryService)
                            b.DisableDeliveryService();
                    });

                    if (rabbitMqOptions.DisableInboxCleanupService)
                        efConfig.DisableInboxCleanupService();
                });

                // to avoid nullability if no consumer added
                Action<IBusRegistrationConfigurator>? busConfigurators = BusConfiguration.GetBusConfigurators();
                if (busConfigurators is not null)
                {
                    busConfigurators(options);
                }

                options.UsingRabbitMq((context, config) =>
                {
                    Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext>? busFactoryConfigurators = BusConfiguration.GetBusFactoryConfigurators();
                    if (busFactoryConfigurators is not null)
                    {
                        busFactoryConfigurators(config, context);
                    }

                    // Add global logging filter (similar to MediatR LoggingBehavior)
                    // Automatically logs all consumer activity with correlation IDs, timing, and errors
                    config.UseConsumeFilter(typeof(LoggingConsumeFilter<>), context);

                    // applies to all receive endpoints
                    config.AutoStart = rabbitMqOptions.AutoStart;
                    config.AutoDelete = rabbitMqOptions.AutoDelete;

                    config.Host(new Uri(rabbitMqOptions.AddressUri), host =>
                    {
                        host.Username(rabbitMqOptions.UserName);
                        host.Password(rabbitMqOptions.Password);
                    });
                    config.ConfigureEndpoints(context);
                });
            });

        return builder;
    }

    private static TBuilder ConfigureLogging<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // Bind configs with safe defaults (nullability-friendly)
        LoggingAndMonitoringOptions? loggingAndMonitoringOptions = builder
            .Configuration
            .GetSection(LoggingAndMonitoringOptions.ConfigurationKey)
            .Get<LoggingAndMonitoringOptions>();

        if (loggingAndMonitoringOptions is null || !loggingAndMonitoringOptions.IsEnabled)
            return builder;

        LoggingOptions loggingConfig = loggingAndMonitoringOptions.Logging;
        ElasticsearchOptions elasticsearchConfig = loggingAndMonitoringOptions.Elasticsearch;

        // Decide a reliable logs folder default
        string logsFolder = string.IsNullOrWhiteSpace(loggingConfig.FolderPath)
            ? Path.Combine(AppContext.BaseDirectory, "Logs")
            : loggingConfig.FolderPath;
        Directory.CreateDirectory(logsFolder);

        // Sanitize application name for file system usage
        string appName = string.IsNullOrWhiteSpace(loggingConfig.Application)
            ? "App"
            : string.Concat(loggingConfig.Application.Split(Path.GetInvalidFileNameChars()));
        string logFilePath = Path.Combine(logsFolder, $"{appName}-.json");

        // Check if this is a WebApplicationBuilder (which has Host property)
        if (builder is WebApplicationBuilder webAppBuilder)
        {
            webAppBuilder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    // Let appsettings.json override/extend
                    .ReadFrom.Configuration(context.Configuration)

                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Environment", loggingAndMonitoringOptions.Environment)
                    .Enrich.WithProperty("Project", loggingConfig.Project)
                    .Enrich.WithProperty("Application", loggingConfig.Application)

                    // Enrich with OpenTelemetry trace context (per log event, reads Activity.Current dynamically)
                    .Enrich.With<ActivityContextEnricher>()

                    .Enrich.WithExceptionDetails()
                    .Enrich.WithSensitiveDataMasking(options =>
                    {
                        options.MaskingOperators.Clear();

                        options.MaskProperties.Add(MaskProperty.WithDefaults("Password"));
                        options.MaskProperties.Add(MaskProperty.WithDefaults("Pwd"));
                        options.MaskProperties.Add(MaskProperty.WithDefaults("Passphrase"));
                        options.MaskProperties.Add(MaskProperty.WithDefaults("Token"));
                        options.MaskProperties.Add(MaskProperty.WithDefaults("ApiKey"));

                        options.MaskValue = new string('*', 10);
                    });

                // Console sink
                if (loggingConfig.EnableConsoleLogs)
                    loggerConfiguration.WriteTo.Async(a => a.Console());

                // Rolling JSON file logs (conditional)
                if (loggingConfig.EnableFileLogs)
                {
                    loggerConfiguration.WriteTo.Async(a => a.File(
                        path: logFilePath,
                        formatter: new CompactJsonFormatter(),
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        retainedFileCountLimit: null,
                        fileSizeLimitBytes: 5 * 1024 * 1024),
                        blockWhenFull: false);
                }

                // Elasticsearch sink (Serilog → Elasticsearch for log storage)
                if (elasticsearchConfig.IsEnabled &&
                    !string.IsNullOrWhiteSpace(elasticsearchConfig.AddressUri))
                {
                    loggerConfiguration.WriteTo.Async(a => a.Elasticsearch(
                        [new Uri(elasticsearchConfig.AddressUri)],
                        opts =>
                        {
                            // Use data streams for modern log management
                            opts.DataStream = new DataStreamName(
                                "logs",
                                loggingConfig.Application.ToLowerInvariant(),
                                loggingAndMonitoringOptions.Environment.ToLowerInvariant()
                            );

                            // Skip ECS template bootstrapping (Elasticsearch 8.11 compatibility)
                            // The data stream will be created automatically on first write
                            opts.BootstrapMethod = BootstrapMethod.Silent;

                            // Configure buffering for better performance
                            opts.ConfigureChannel = channelOpts =>
                            {
                                channelOpts.BufferOptions = new BufferOptions
                                {
                                    ExportMaxConcurrency = 10
                                };
                            };
                        },
                        // Configure transport (connection settings)
                        transport =>
                        {
                            // Add API key authentication if provided
                            if (!string.IsNullOrWhiteSpace(elasticsearchConfig.ApiKey))
                            {
                                transport.Authentication(new ApiKey(elasticsearchConfig.ApiKey));
                            }
                            else if (!string.IsNullOrWhiteSpace(elasticsearchConfig.Username))
                            {
                                transport.Authentication(new BasicAuthentication(elasticsearchConfig.Username, elasticsearchConfig.Password ?? string.Empty));
                            }
                        }));
                }
            }, preserveStaticLogger: true, writeToProviders: true);
        }

        return builder;
    }

    private static TBuilder ConfigureObservability<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        LoggingAndMonitoringOptions? loggingAndMonitoringOptions = builder
            .Configuration
            .GetSection(LoggingAndMonitoringOptions.ConfigurationKey)
            .Get<LoggingAndMonitoringOptions>();

        if (loggingAndMonitoringOptions is null ||
            !loggingAndMonitoringOptions.IsEnabled ||
            !loggingAndMonitoringOptions.Elasticsearch.IsEnabled)
            return builder;

        OpenTelemetryOptions otelConfig = loggingAndMonitoringOptions.OpenTelemetry;
        ApmOptions apmConfig = loggingAndMonitoringOptions.Apm;

        if (!otelConfig.EnableTracing && !otelConfig.EnableMetrics)
            return builder;

        // Configure OpenTelemetry Resource (shared by both tracing and metrics)
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: apmConfig.ServiceName,
                    serviceVersion: apmConfig.ServiceVersion,
                    serviceInstanceId: Environment.MachineName)
                .AddAttributes([
                    new("deployment.environment", loggingAndMonitoringOptions.Environment),
                    new("service.namespace", loggingAndMonitoringOptions.Logging.Project)
                ]));

        // Configure OpenTelemetry Tracing
        // Exports traces to APM Server via OTLP protocol
        if (otelConfig.EnableTracing)
        {
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing
                        // ASP.NET Core instrumentation
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;

                            // Filter out health check endpoints and enrich with custom tags
                            options.Filter = httpContext =>
                            {
                                // Add user information if available
                                if (httpContext.User?.Identity?.IsAuthenticated == true)
                                {
                                    Activity.Current?.SetTag("user.id", httpContext.User.Identity.Name);
                                }

                                return !httpContext.Request.Path.StartsWithSegments("/health") &&
                                       !httpContext.Request.Path.StartsWithSegments("/ping");
                            };

                            // Enrich spans with additional request information
                            options.EnrichWithHttpRequest = (activity, request) =>
                            {
                                // The CorrelationIdMiddleware already sets this, but ensure it's present
                                // This is important for APM correlation
                                if (request.HttpContext.Items.TryGetValue(IdGeneratorService.CorrelationIdKey, out object? correlationId))
                                {
                                    activity.SetTag(IdGeneratorService.CorrelationIdKey, correlationId?.ToString());
                                }
                            };
                        })

                        // HTTP Client instrumentation (outbound calls)
                        .AddHttpClientInstrumentation(options =>
                        {
                            options.RecordException = true;
                            options.EnrichWithHttpRequestMessage = (activity, request) =>
                            {
                                activity.SetTag("http.request.method", request.Method.Method);
                            };
                        })

                        // NOTE: SQL instrumentation is disabled here to avoid conflicts with Elastic APM
                        // Elastic APM provides superior EF Core instrumentation with query details
                        // If you want to use OpenTelemetry for SQL instead, comment out AddAllElasticApm() below
                        //.AddSqlClientInstrumentation()

                        // MassTransit instrumentation (message bus tracing)
                        .AddSource("MassTransit")

                        // Your custom Activity source (from CustomExceptionHandler)
                        .AddSource(apmConfig.ServiceName);

                    // Configure sampler based on configuration
                    if (otelConfig.TraceSampler == "AlwaysOn")
                    {
                        tracing.SetSampler(new AlwaysOnSampler());
                    }
                    else if (otelConfig.TraceSampler == "AlwaysOff")
                    {
                        tracing.SetSampler(new AlwaysOffSampler());
                    }

                    // Export to Elastic APM via OTLP
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otelConfig.AddressUri);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
                });
        }

        // Configure OpenTelemetry Metrics
        // Exports metrics to APM Server via OTLP protocol
        if (otelConfig.EnableMetrics)
        {
            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics
                        // ASP.NET Core metrics
                        .AddAspNetCoreInstrumentation()

                        // HTTP Client metrics
                        .AddHttpClientInstrumentation()

                        // Runtime metrics (GC, ThreadPool, etc.)
                        .AddRuntimeInstrumentation()

                        // MassTransit metrics
                        .AddMeter("MassTransit");

                    // Export to Elastic APM via OTLP
                    metrics.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otelConfig.AddressUri);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
                });
        }

        // Add Elastic APM .NET Agent (standalone mode)
        // Provides native Elasticsearch/Kibana APM integration with enhanced features:
        // - Automatic instrumentation for EF Core, HttpClient, etc.
        // - Better error tracking and context capture
        // - Full stack traces and exception details
        // - Works alongside OpenTelemetry for dual observability
        if (apmConfig.TransactionSampleRate > 0)
            builder.Services.AddAllElasticApm();

        return builder;
    }

    private static TBuilder ConfigureAdvancedHealthChecks<TBuilder>(this TBuilder builder)
    where TBuilder : IHostApplicationBuilder
    {
        RabbitMqOptions? rabbitMqOptions = builder
            .Configuration
            .GetSection(RabbitMqOptions.ConfigurationKey)
            .Get<RabbitMqOptions>();

        ApplicationOptions? applicationOptions = builder
            .Configuration
            .GetSection(ApplicationOptions.ConfigurationKey)
            .Get<ApplicationOptions>();

        LoggingAndMonitoringOptions? loggingOptions = builder
            .Configuration
            .GetSection(LoggingAndMonitoringOptions.ConfigurationKey)
            .Get<LoggingAndMonitoringOptions>();

        KeycloakOptions? keycloakOptions = builder
            .Configuration
            .GetSection(KeycloakOptions.ConfigurationKey)
            .Get<KeycloakOptions>();

        VaultSettingOptions? vaultOptions = builder
            .Configuration
            .GetSection(VaultSettingOptions.ConfigurationKey)
            .Get<VaultSettingOptions>();

        RedisOptions? redisOptions = builder
            .Configuration
            .GetSection(RedisOptions.ConfigurationKey)
            .Get<RedisOptions>();

        IHealthChecksBuilder healthChecks = builder.Services.AddHealthChecks();

        if (rabbitMqOptions is not null &&
            applicationOptions is not null &&
            rabbitMqOptions.IsEnabled)
        {
            healthChecks
                .AddRabbitMQ(
                    sp =>
                    {
                        ConnectionFactory factory = new()
                        {
                            Uri = new Uri(rabbitMqOptions.AmqpAddressUri),
                            UserName = rabbitMqOptions.UserName,
                            Password = rabbitMqOptions.Password
                        };
                        return factory.CreateConnectionAsync().GetAwaiter().GetResult();
                    },
                    name: "rabbitmq",
                    failureStatus: HealthStatus.Degraded,
                    tags: [ReadyTag, "messaging", "rabbitmq"]);

            // Add database health check based on configured provider
            if (applicationOptions.DbProviderType == DatabaseProviderType.PostgreSql)
            {
                healthChecks.AddNpgSql(
                    connectionString: applicationOptions.DbConnection,
                    name: "npgsql-outbox",
                    failureStatus: HealthStatus.Degraded,
                    tags: [ReadyTag, "database", "npgsql", "outbox"]);
            }
            else
            {
                healthChecks.AddSqlServer(
                    connectionString: applicationOptions.DbConnection,
                    name: "sqlserver-outbox",
                    failureStatus: HealthStatus.Degraded,
                    tags: [ReadyTag, "database", "sqlserver", "outbox"]);
            }
        }

        // Add Redis health check
        if (redisOptions?.AddressUri is not null &&
            redisOptions?.IsEnabled == true)
        {
            healthChecks
            .AddRedis(
                redisOptions.AddressUri,
                name: "redis",
                failureStatus: HealthStatus.Degraded,
                tags: [ReadyTag, "cache", "redis"]);
        }

        if (keycloakOptions?.HealthCheckAddressUri is not null)
        {
            // The Docs state /health/ready is the correct endpoint for "Readiness"
            string keycloakHealthUrl = keycloakOptions.HealthCheckAddressUri.TrimEnd('/');

            healthChecks.AddUrlGroup(
                new Uri(keycloakHealthUrl),
                name: "keycloak",
                failureStatus: HealthStatus.Degraded,
                tags: [ReadyTag, "authentication", "keycloak"],
                timeout: TimeSpan.FromSeconds(5));
        }

        // Elasticsearch Health Check (if enabled)
        if (loggingOptions?.Elasticsearch?.IsEnabled == true &&
            !string.IsNullOrWhiteSpace(loggingOptions.Elasticsearch.AddressUri))
        {
            string elasticsearchHealthUrl = $"{loggingOptions.Elasticsearch.AddressUri}/_cluster/health";

            healthChecks.AddUrlGroup(
                new Uri(elasticsearchHealthUrl),
                name: "elasticsearch",
                failureStatus: HealthStatus.Degraded,
                tags: [ReadyTag, "logging", "elasticsearch"],
                timeout: TimeSpan.FromSeconds(5));
        }

        // HashiCorp Vault Health Check (if enabled)
        if (vaultOptions?.IsEnabled == true &&
            !string.IsNullOrWhiteSpace(vaultOptions.AddressUri))
        {
            string vaultHealthUrl = $"{vaultOptions.AddressUri}/v1/sys/health";

            healthChecks.AddUrlGroup(
                new Uri(vaultHealthUrl),
                name: "vault",
                failureStatus: HealthStatus.Degraded,
                tags: [ReadyTag, "secrets", "vault"],
                timeout: TimeSpan.FromSeconds(5));
        }

        // Disk Space Health Check
        // Check if C: drive (Windows) or / (Linux) has at least 1 GB free
        string driveName = OperatingSystem.IsWindows() ? "C:\\" : "/";
        healthChecks.AddDiskStorageHealthCheck(
            options => options.AddDrive(driveName, minimumFreeMegabytes: 1024), // 1 GB
            name: "disk-space",
            failureStatus: HealthStatus.Degraded,
            tags: [ReadyTag, "system", "disk"]);

        // Memory Health Check
        // Warn if available memory < 512 MB
        healthChecks.AddPrivateMemoryHealthCheck(
            maximumMemoryBytes: 1024 * 1024 * 1024, // 1 GB maximum
            name: "memory",
            failureStatus: HealthStatus.Degraded,
            tags: [ReadyTag, "system", "memory"]);

        return builder;
    }

    private static AppSettingsKeyManagement ConfigureAppSettings(IConfiguration configuration)
    {
        ApplicationOptions? applicationOptions = configuration
            .GetSection(ApplicationOptions.ConfigurationKey)
            .Get<ApplicationOptions>();
        AzurBlobServiceOptions? azurBlobServiceOptions = configuration
            .GetSection(AzurBlobServiceOptions.ConfigurationKey)
            .Get<AzurBlobServiceOptions>();
        CorsConfigurationOptions? corsConfigurationOptions = configuration
            .GetSection(CorsConfigurationOptions.ConfigurationKey)
            .Get<CorsConfigurationOptions>();
        HangfireOptions? hangfireOptions = configuration
            .GetSection(HangfireOptions.ConfigurationKey)
            .Get<HangfireOptions>();
        IntegrationApisOptions? integrationApisOptions = configuration
            .GetSection(IntegrationApisOptions.ConfigurationKey)
            .Get<IntegrationApisOptions>();
        KeycloakOptions? keycloakOptions = configuration
            .GetSection(KeycloakOptions.ConfigurationKey)
            .Get<KeycloakOptions>();
        LoggingAndMonitoringOptions? loggingAndMonitoringOptions = configuration
            .GetSection(LoggingAndMonitoringOptions.ConfigurationKey)
            .Get<LoggingAndMonitoringOptions>();
        MobileNotificationOptions? mobileNotificationOptions = configuration
            .GetSection(MobileNotificationOptions.ConfigurationKey)
            .Get<MobileNotificationOptions>();
        RabbitMqOptions? rabbitMqOptions = configuration
            .GetSection(RabbitMqOptions.ConfigurationKey)
            .Get<RabbitMqOptions>();
        RedisOptions? redisOptions = configuration
            .GetSection(RedisOptions.ConfigurationKey)
            .Get<RedisOptions>();
        SmtpEmailOptions? smtpEmailOptions = configuration
            .GetSection(SmtpEmailOptions.ConfigurationKey)
            .Get<SmtpEmailOptions>();
        VaultSettingOptions? vaultSettingOptions = configuration
            .GetSection(VaultSettingOptions.ConfigurationKey)
            .Get<VaultSettingOptions>();

        if (applicationOptions is null ||
            azurBlobServiceOptions is null ||
            corsConfigurationOptions is null ||
            hangfireOptions is null ||
            keycloakOptions is null ||
            loggingAndMonitoringOptions is null ||
            mobileNotificationOptions is null ||
            rabbitMqOptions is null ||
            redisOptions is null ||
            smtpEmailOptions is null ||
            vaultSettingOptions is null)
        {
            throw new InvalidOperationException("One or more configuration sections are missing or invalid.");
        }

        return new AppSettingsKeyManagement(applicationOptions,
                                            azurBlobServiceOptions,
                                            corsConfigurationOptions,
                                            hangfireOptions,
                                            integrationApisOptions,
                                            keycloakOptions,
                                            loggingAndMonitoringOptions,
                                            mobileNotificationOptions,
                                            rabbitMqOptions,
                                            redisOptions,
                                            smtpEmailOptions,
                                            vaultSettingOptions);
    }

    private static void MapHealthChecks(this WebApplication app)
    {
        // Map comprehensive health check endpoint
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Map readiness probe endpoint (for Kubernetes/container orchestration)
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains(ReadyTag),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Map liveness probe endpoint (for Kubernetes/container orchestration)
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false, // Liveness probe - just checks if app is running
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }

    private static IApplicationBuilder UseCorsIfEnabled(this IApplicationBuilder builder, CorsConfigurationOptions corsOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder is WebApplication webApp &&
            (corsOptions is not null && corsOptions.IsEnabled))
            webApp.UseCors(corsOptions.PolicyName);

        return builder;
    }

    #endregion
}
