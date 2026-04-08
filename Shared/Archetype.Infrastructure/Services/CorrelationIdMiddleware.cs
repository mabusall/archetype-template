namespace Archetype.Infrastructure.Services;

/// <summary>
/// Middleware that ensures every request has a correlation id and exposes it to logging and tracing.
/// </summary>
public sealed class CorrelationIdMiddleware(RequestDelegate next,
                                            ILogger<CorrelationIdMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Read correlation id from header or generate new one
        if (!context.Request.Headers.TryGetValue(IdGeneratorService.CorrelationIdKey, out StringValues correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        string correlationIdValue = correlationId.ToString();

        // Store it on HttpContext for later use (e.g. ICorrelationIdService)
        context.Items[IdGeneratorService.CorrelationIdKey] = correlationIdValue;

        // Add correlation ID to response headers for client-side tracing
        context.Response.Headers.TryAdd(IdGeneratorService.CorrelationIdKey, correlationIdValue);

        // Add as trace tags for OpenTelemetry / APM
        Activity? currentActivity = Activity.Current;
        if (currentActivity is not null)
        {
            // Set as activity tag for OpenTelemetry
            currentActivity.SetTag(IdGeneratorService.CorrelationIdKey, correlationIdValue);

            // Set as baggage for distributed tracing propagation
            currentActivity.SetBaggage(IdGeneratorService.CorrelationIdKey, correlationIdValue);
        }

        // Set as Elastic APM transaction label (makes it searchable in APM with labels.SystemCorrelationId)
        Elastic.Apm.Agent.Tracer.CurrentTransaction?.SetLabel(IdGeneratorService.CorrelationIdKey, correlationIdValue);

        // Add it to logging scope (replacement for Serilog LogContext.PushProperty)
        using (logger.BeginScope(new Dictionary<string, object?>
        {
            [IdGeneratorService.CorrelationIdKey] = correlationIdValue
        }))
        {
            await next(context);
        }
    }
}
