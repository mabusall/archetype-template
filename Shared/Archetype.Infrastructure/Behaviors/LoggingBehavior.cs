namespace Archetype.Infrastructure.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger,
                                                  ICurrentUserService currentUserService,
                                                  ICorrelationIdService correlationIdService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request,
                                        RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        string userId = currentUserService.UserId ?? string.Empty;
        string correlationId = correlationIdService.Get() ?? string.Empty;
        string requestName = typeof(TRequest).Name;

        // Capture memory before request (cumulative allocations - immune to GC)
        long allocatedBytesBefore = GC.GetTotalAllocatedBytes(precise: false);

        // Create OpenTelemetry span for MediatR request
        using Activity? activity = Activity.Current?.Source.StartActivity($"Archetype.Mediator {requestName}", ActivityKind.Internal);

        // Enrich activity with correlation and user context
        if (activity is not null)
        {
            activity.SetTag("messaging.system", "Archetype.Mediator");
            activity.SetTag("messaging.operation", "process");
            activity.SetTag("messaging.message_type", requestName);

            if (!string.IsNullOrEmpty(correlationId))
            {
                activity.SetTag(IdGeneratorService.CorrelationIdKey, correlationId);
                // Set as Elastic APM span label
                Elastic.Apm.Agent.Tracer.CurrentSpan?.SetLabel(IdGeneratorService.CorrelationIdKey, correlationId);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                activity.SetTag("user.id", userId);
                Elastic.Apm.Agent.Tracer.CurrentSpan?.SetLabel("user.id", userId);
            }
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("[START] {RequestName} [UserId: {UserId}]",
                                  requestName,
                                  userId);
        }

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("RequestData={@RequestData}", request);
        }

        Stopwatch sw = Stopwatch.StartNew();
        TResponse? response = default;

        try
        {
            response = await next(cancellationToken);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            if (activity is not null)
            {
                activity.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity.AddException(ex);
            }

            logger.LogError(ex,
                            "[ERROR] {RequestName} failed after {ElapsedSeconds:F2}s with exception: {ExceptionMessage}",
                            requestName,
                            sw.Elapsed.TotalSeconds,
                            ex.Message);
            throw;
        }
        finally
        {
            sw.Stop();
        }

        double elapsedSeconds = sw.Elapsed.TotalSeconds;

        // Capture memory after request (cumulative allocations)
        long allocatedBytesAfter = GC.GetTotalAllocatedBytes(precise: false);

        // Calculate memory allocated (always positive, immune to GC)
        long memoryAllocated = allocatedBytesAfter - allocatedBytesBefore;
        double memoryAllocatedMB = memoryAllocated / 1024.0 / 1024.0;

        // Add memory metrics to activity (OpenTelemetry tags)
        if (activity is not null)
        {
            activity.SetTag("request.duration_seconds", elapsedSeconds);
            activity.SetTag("request.memory_mb", memoryAllocatedMB);

            // Set as Elastic APM span labels (searchable in Kibana)
            // Note: "labels." prefix is added by Elastic APM automatically
            Elastic.Apm.Agent.Tracer.CurrentSpan?.SetLabel("request.memory_mb", memoryAllocatedMB.ToString("F2"));
        }

        if (elapsedSeconds > 3 && logger.IsEnabled(LogLevel.Warning))
        {
            logger.LogWarning("[PERFORMANCE] {RequestName} took {ElapsedSeconds:F2}s",
                              requestName,
                              elapsedSeconds);
        }

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("ResponseData={@ResponseData}", response);
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("[END] {RequestName} handled in {ElapsedSeconds:F2}s, Memory: {MemoryKB:F2} KB",
                                  requestName,
                                  elapsedSeconds,
                                  memoryAllocated / 1024.0);
        }

        return response!;
    }
}