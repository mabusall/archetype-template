namespace Archetype.Infrastructure.Database;

public static class DbContextExtension
{
    extension(WebApplication app)
    {
        /// <summary>
        /// Apply any pending EF Core migrations during application startup (synchronous).
        /// </summary>
        public WebApplication UseMigration<TContext>()
            where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(app);

            using IServiceScope scope = app.Services.CreateScope();
            Microsoft.Extensions.Logging.ILogger? logger = scope
                .ServiceProvider
                .GetService<ILoggerFactory>()?
                .CreateLogger(typeof(DbContextExtension));

            try
            {
                TContext? context = scope.ServiceProvider.GetService<TContext>();

                if (context is null)
                {
                    if (logger?.IsEnabled(LogLevel.Warning) == true)
                    {
                        logger.LogWarning("DbContext {DbContext} is not registered. Skipping migration.", typeof(TContext).Name);
                    }
                    return app;
                }

                List<string> pendingMigrations = context.Database.GetPendingMigrations().ToList();
                if (pendingMigrations.Count == 0)
                {
                    if (logger?.IsEnabled(LogLevel.Information) == true)
                    {
                        logger.LogInformation("No pending migrations for {DbContext}.", typeof(TContext).Name);
                    }
                    return app;
                }

                if (logger?.IsEnabled(LogLevel.Information) == true)
                {
                    logger.LogInformation("Applying {Count} migration(s) for {DbContext}...", pendingMigrations.Count, typeof(TContext).Name);
                }

                context.Database.Migrate();

                if (logger?.IsEnabled(LogLevel.Information) == true)
                {
                    logger.LogInformation("Successfully applied {Count} migration(s) for {DbContext}.", pendingMigrations.Count, typeof(TContext).Name);
                }
            }
            catch (Exception ex)
            {
                if (logger?.IsEnabled(LogLevel.Error) == true)
                {
                    logger.LogError(ex, "Failed to apply migrations for {DbContext}. Application will continue.", typeof(TContext).Name);
                }
            }

            return app;
        }

        /// <summary>
        /// Apply any pending EF Core migrations during application startup (asynchronous).
        /// Preferred for non-blocking startup flows.
        /// </summary>
        public async Task<WebApplication> UseMigrationAsync<TContext>(CancellationToken cancellationToken = default)
            where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(app);

            await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
            Microsoft.Extensions.Logging.ILogger? logger = scope
                .ServiceProvider
                .GetService<ILoggerFactory>()?
                .CreateLogger(typeof(DbContextExtension));

            try
            {
                TContext? context = scope.ServiceProvider.GetService<TContext>();

                if (context is null)
                {
                    if (logger?.IsEnabled(LogLevel.Warning) == true)
                    {
                        logger.LogWarning("DbContext {DbContext} is not registered. Skipping migration.", typeof(TContext).Name);
                    }
                    return app;
                }

                if (!await context.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (logger?.IsEnabled(LogLevel.Warning) == true)
                    {
                        logger.LogWarning("Cannot connect to database for {DbContext}. Skipping migration.", typeof(TContext).Name);
                    }
                    return app;
                }

                List<string> pendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).ToList();
                if (pendingMigrations.Count == 0)
                {
                    if (logger?.IsEnabled(LogLevel.Information) == true)
                    {
                        logger.LogInformation("No pending migrations for {DbContext}.", typeof(TContext).Name);
                    }
                    return app;
                }

                if (logger?.IsEnabled(LogLevel.Information) == true)
                {
                    logger.LogInformation("Applying {Count} migration(s) (async) for {DbContext}...", pendingMigrations.Count, typeof(TContext).Name);
                }

                await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

                if (logger?.IsEnabled(LogLevel.Information) == true)
                {
                    logger.LogInformation("Successfully applied {Count} migration(s) (async) for {DbContext}.", pendingMigrations.Count, typeof(TContext).Name);
                }
            }
            catch (Exception ex)
            {
                if (logger?.IsEnabled(LogLevel.Error) == true)
                {
                    logger.LogError(ex, "Failed to apply migrations (async) for {DbContext}. Application will continue.", typeof(TContext).Name);
                }
            }

            return app;
        }
    }
}