namespace Archetype.Infrastructure.Extensions;

public static class WebExtensions
{
    extension<TBuilder>(TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        public TBuilder RequireApiKey()
        {
            ArgumentNullException.ThrowIfNull(builder);

            // If ApiKeyFilter is stateless you could cache it, but this is fine.
            return builder.AddEndpointFilter(new ApiKeyFilter());
        }

        public TBuilder RequirePermissions<TPermission>(params TPermission[] requiredPermissions)
            where TPermission : Enum
        {
            ArgumentNullException.ThrowIfNull(builder);

            if (requiredPermissions is null || requiredPermissions.Length == 0)
                throw new ArgumentException("At least one permission is required.", nameof(requiredPermissions));

            // Convert enum values to int safely
            // Note: handles any underlying enum type by converting through Int64 then checked-casting to int.
            int[] permissionValues = requiredPermissions
                .Select(p => checked((int)Convert.ToInt64(p)))
                .ToArray();

            return builder.AddEndpointFilter(new PermissionFilter(permissionValues));
        }
    }

    extension(IEndpointRouteBuilder endpoints)
    {
        /// <summary>
        /// Adds a '/Ping' endpoint that responds with a simple ping message and the current UTC time.
        /// </summary>
        /// <remarks>
        /// The '/Ping' endpoint is accessible without authentication and returns a plain text response
        /// containing the word 'Ping' and the current UTC timestamp. Useful for health checks.
        /// </remarks>
        public void MapPing()
        {
            ArgumentNullException.ThrowIfNull(endpoints);

            endpoints.MapGet("/Ping", async context =>
            {
                string response = $"Ping on {DateTime.UtcNow:O}";

                context.Response.StatusCode = StatusCodes.Status200OK;

                // Response is plain text, so use text/plain (not application/json).
                context.Response.ContentType = $"{MediaTypeNames.Text.Plain}; charset=utf-8";

                // Respect request cancellation if the client disconnects.
                await context.Response.WriteAsync(response, context.RequestAborted);
            })
            .AllowAnonymous();
        }
    }

    extension(HttpContext httpContext)
    {
        /// <summary>
        /// Extracts the bearer access token from the Authorization header.
        /// Returns null if missing or not a bearer token.
        /// </summary>
        public string? GetAccessToken()
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            if (!httpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues))
                return null;

            string value = headerValues.ToString();
            if (string.IsNullOrWhiteSpace(value))
                return null;

            const string prefix = "Bearer ";

            if (!value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return null;

            string token = value[prefix.Length..].Trim();
            return token.Length == 0 ? null : token;
        }

        /// <summary>
        /// Reads the correlation id.
        /// 1) Tries <see cref="HttpContext.Items"/> (set by middleware).
        /// 2) Falls back to incoming request header (optional but common in distributed tracing).
        /// Returns empty string if not found.
        /// </summary>
        public string GetCorrelationId()
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            // Preferred source: middleware-populated Items
            if (httpContext.Items.TryGetValue(IdGeneratorService.CorrelationIdKey, out object? value) &&
                value is string s &&
                !string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            // Fallback: incoming request header (in case middleware didn't run yet)
            if (httpContext.Request.Headers.TryGetValue(IdGeneratorService.CorrelationIdKey, out StringValues header) &&
                !string.IsNullOrWhiteSpace(header))
            {
                return header.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Same as <see cref="GetCorrelationId"/> but returns null when absent.
        /// Useful when you want to distinguish "missing" from "empty".
        /// </summary>
        public string? TryGetCorrelationId()
        {
            string id = httpContext.GetCorrelationId();
            return string.IsNullOrWhiteSpace(id) ? null : id;
        }
    }

    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseCorrelationId()
        {
            ArgumentNullException.ThrowIfNull(app);
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }

        public IApplicationBuilder ConfigureRequestTimeout(TimeSpan timeout)
        {
            ArgumentNullException.ThrowIfNull(app);

            if (timeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than zero.");

            app.Use(async (context, next) =>
            {
                CancellationToken originalToken = context.RequestAborted;

                using CancellationTokenSource timeoutCts = new(timeout);
                using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(originalToken, timeoutCts.Token);

                context.RequestAborted = linkedCts.Token;

                try
                {
                    await next(context);
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !originalToken.IsCancellationRequested)
                {
                    // Timeout happened (not a client disconnect).
                    if (!context.Response.HasStarted)
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
                    }
                    // If response already started, just let it abort naturally.
                }
                finally
                {
                    // Restore the original token to avoid surprising downstream/after-next logic.
                    context.RequestAborted = originalToken;
                }
            });

            return app;
        }
    }

    extension(HttpRequestHeaders headers)
    {
        /// <summary>
        /// Adds the correlation id to outgoing HTTP headers (for HttpClient, etc.).
        /// </summary>
        public void AddCorrelationId(string correlationId)
        {
            ArgumentNullException.ThrowIfNull(headers);

            if (string.IsNullOrWhiteSpace(correlationId))
                return;

            // Remove any existing value to avoid duplicates, then add the current one.
            headers.Remove(IdGeneratorService.CorrelationIdKey);
            headers.TryAddWithoutValidation(IdGeneratorService.CorrelationIdKey, correlationId);
        }

        /// <summary>
        /// Adds the correlation id only if missing (doesn't overwrite).
        /// Handy if some upstream code already set it.
        /// </summary>
        public void AddCorrelationIdIfMissing(string correlationId)
        {
            ArgumentNullException.ThrowIfNull(headers);

            if (string.IsNullOrWhiteSpace(correlationId))
                return;

            if (!headers.Contains(IdGeneratorService.CorrelationIdKey))
            {
                headers.TryAddWithoutValidation(IdGeneratorService.CorrelationIdKey, correlationId);
            }
        }
    }
}