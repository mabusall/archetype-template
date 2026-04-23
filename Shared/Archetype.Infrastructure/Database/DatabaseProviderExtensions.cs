namespace Archetype.Infrastructure.Database;

public static class DatabaseProviderExtensions
{
    extension(DbContextOptionsBuilder options)
    {
        public DbContextOptionsBuilder UseConfiguredProvider(ApplicationOptions applicationOptions,
                                                             Action<SqlServerDbContextOptionsBuilder>? sqlServerOptions = null,
                                                             Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptions = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(applicationOptions.DbConnection);

            return applicationOptions.DbProviderType switch
            {
                DatabaseProviderType.SqlServer => EntityFramework.Exceptions.SqlServer.ExceptionProcessorExtensions
                    .UseExceptionProcessor(
                        options.UseSqlServer(applicationOptions.DbConnection, sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 10,
                                maxRetryDelay: TimeSpan.FromSeconds(5),
                                errorNumbersToAdd: null);
                            sqlServerOptions?.Invoke(sqlOptions);
                        })),

                DatabaseProviderType.PostgreSql => EntityFramework.Exceptions.PostgreSQL.ExceptionProcessorExtensions
                    .UseExceptionProcessor(
                        options.UseNpgsql(applicationOptions.DbConnection, pgOptions =>
                        {
                            pgOptions.EnableRetryOnFailure(
                                maxRetryCount: 10,
                                maxRetryDelay: TimeSpan.FromSeconds(5),
                                errorCodesToAdd: null);
                            npgsqlOptions?.Invoke(pgOptions);
                        })),

                _ => throw new InvalidOperationException(
                    $"Unsupported database provider: '{applicationOptions.DbProviderType}'. " +
                    $"Supported values are: {string.Join(", ", Enum.GetNames<DatabaseProviderType>())}.")
            };
        }
    }

    extension(IGlobalConfiguration config)
    {
        public IGlobalConfiguration UseConfiguredHangfireStorage(ApplicationOptions applicationOptions,
                                                                  HangfireOptions hangfireOptions)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(applicationOptions.DbConnection);

            return applicationOptions.DbProviderType switch
            {
                DatabaseProviderType.SqlServer => config.UseSqlServerStorage(applicationOptions.DbConnection, new SqlServerStorageOptions
                {
                    SchemaName = hangfireOptions.SchemaName,
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromSeconds(5),
                }),

                DatabaseProviderType.PostgreSql => config.UsePostgreSqlStorage(options =>
                    options.UseNpgsqlConnection(applicationOptions.DbConnection), new PostgreSqlStorageOptions
                    {
                        SchemaName = hangfireOptions.SchemaName,
                        QueuePollInterval = TimeSpan.FromSeconds(5),
                    }),

                _ => throw new InvalidOperationException(
                    $"Unsupported database provider: '{applicationOptions.DbProviderType}'. " +
                    $"Supported values are: {string.Join(", ", Enum.GetNames<DatabaseProviderType>())}.")
            };
        }
    }

    extension(IEntityFrameworkOutboxConfigurator configurator)
    {
        public void ConfigureMassTransitOutbox(ApplicationOptions applicationOptions)
        {
            switch (applicationOptions.DbProviderType)
            {
                case DatabaseProviderType.SqlServer:
                    configurator.UseSqlServer();
                    break;

                case DatabaseProviderType.PostgreSql:
                    configurator.UsePostgres();
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported database provider: '{applicationOptions.DbProviderType}'. " +
                        $"Supported values are: {string.Join(", ", Enum.GetNames<DatabaseProviderType>())}.");
            }
        }
    }
}
