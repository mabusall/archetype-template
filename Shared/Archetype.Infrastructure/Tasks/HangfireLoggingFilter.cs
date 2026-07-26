namespace Archetype.Infrastructure.Tasks;

/// <summary>
/// Hangfire filter that logs all job activity globally (similar to MediatR LoggingBehavior and MassTransit LoggingConsumeFilter).
/// Automatically applied to all jobs for centralized logging with correlation IDs, timing, and exception tracking.
/// </summary>
public class HangfireLoggingFilter(ILogger<HangfireLoggingFilter> logger,
                                   IServiceProvider serviceProvider)
    : IServerFilter
{
    public void OnPerforming(PerformingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string jobType = context.BackgroundJob.Job.Type.Name;
        string jobMethod = context.BackgroundJob.Job.Method.Name;
        string jobId = context.BackgroundJob.Id;

        // Get correlation ID from job parameters or generate new one
        string correlationId = GetOrCreateCorrelationId(context);

        // Store correlation ID in context for use in OnPerformed
        context.Items["CorrelationId"] = correlationId;
        context.Items["StartTime"] = Stopwatch.StartNew();

        // Create OpenTelemetry span for Hangfire job
        Activity? activity = Activity.Current?.Source.StartActivity($"Hangfire {jobType}.{jobMethod}", ActivityKind.Internal);

        // Store activity in context so we can complete it in OnPerformed
        context.Items["Activity"] = activity;

        // Enrich activity with job context
        if (activity is not null)
        {
            activity.SetTag("job.id", jobId);
            activity.SetTag("job.type", jobType);
            activity.SetTag("job.method", jobMethod);
            activity.SetTag("job.queue", context.BackgroundJob.Job.Queue);
            activity.SetTag("job.attempts", context.GetJobParameter<int>("RetryCount"));
            activity.SetTag(IdGeneratorService.CorrelationIdKey, correlationId);

            // Set as Elastic APM span label
            Elastic.Apm.Agent.Tracer.CurrentSpan?.SetLabel(IdGeneratorService.CorrelationIdKey, correlationId);
            Elastic.Apm.Agent.Tracer.CurrentSpan?.SetLabel("job.type", jobType);
        }

        // Add correlation ID to logging scope
        // This ensures all logs within the job have the correlation ID
        IDisposable? logScope = logger.BeginScope(new Dictionary<string, object?>
        {
            [IdGeneratorService.CorrelationIdKey] = correlationId,
            ["JobId"] = jobId,
            ["JobType"] = jobType
        });

        // Store log scope to dispose in OnPerformed
        context.Items["LogScope"] = logScope;

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("[START] Executing Hangfire job {JobType}.{JobMethod} [JobId: {JobId}]",
                                  jobType,
                                  jobMethod,
                                  jobId);
        }

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Job Arguments: {@JobArguments}",
                            context.BackgroundJob.Job.Args);
        }
    }

    public void OnPerformed(PerformedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string jobType = context.BackgroundJob.Job.Type.Name;
        string jobMethod = context.BackgroundJob.Job.Method.Name;
        string jobId = context.BackgroundJob.Id;

        // Retrieve stored values from context
        Stopwatch? sw = context.Items.TryGetValue("StartTime", out object? startTimeObj) && startTimeObj is Stopwatch startTime
            ? startTime
            : null;

        Activity? activity = context.Items.TryGetValue("Activity", out object? activityObj) && activityObj is Activity act
            ? act
            : null;

        IDisposable? logScope = context.Items.TryGetValue("LogScope", out object? scopeObj) && scopeObj is IDisposable scope
            ? scope
            : null;

        sw?.Stop();
        double elapsedSeconds = sw?.Elapsed.TotalSeconds ?? 0;

        try
        {
            if (context.Exception is not null)
            {
                // Job failed with exception
                if (activity is not null)
                {
                    activity.SetStatus(ActivityStatusCode.Error, context.Exception.Message);
                    activity.AddException(context.Exception);
                }

                logger.LogError(context.Exception,
                                "[FAILED] Hangfire job {JobType}.{JobMethod} failed after {ElapsedSeconds:F2}s [JobId: {JobId}]",
                                jobType,
                                jobMethod,
                                elapsedSeconds,
                                jobId);
            }
            else if (context.Canceled)
            {
                // Job was canceled
                activity?.SetStatus(ActivityStatusCode.Error, "Job canceled");

                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("[CANCELED] Hangfire job {JobType}.{JobMethod} was canceled after {ElapsedSeconds:F2}s [JobId: {JobId}]",
                                      jobType,
                                      jobMethod,
                                      elapsedSeconds,
                                      jobId);
                }
            }
            else
            {
                // Job succeeded
                if (activity is not null)
                {
                    activity.SetStatus(ActivityStatusCode.Ok);
                    activity.SetTag("job.duration_seconds", elapsedSeconds);
                }

                // Performance warning if job took too long
                if (elapsedSeconds > 30 && logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("[PERFORMANCE] Hangfire job {JobType}.{JobMethod} took {ElapsedSeconds:F2}s [JobId: {JobId}]",
                                      jobType,
                                      jobMethod,
                                      elapsedSeconds,
                                      jobId);
                }

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("[END] Hangfire job {JobType}.{JobMethod} completed successfully in {ElapsedSeconds:F2}s [JobId: {JobId}]",
                                          jobType,
                                          jobMethod,
                                          elapsedSeconds,
                                          jobId);
                }
            }

            // Log result if available
            if (context.Result != null && logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Job Result: {@JobResult}", context.Result);
            }
        }
        finally
        {
            // Clean up resources
            activity?.Dispose();
            logScope?.Dispose();
        }
    }

    private string GetOrCreateCorrelationId(PerformingContext context)
    {
        // Try to get correlation ID from job arguments
        // Jobs can pass correlation ID as parameter for distributed tracing
        if (context.BackgroundJob.Job.Args.Count > 0)
        {
            foreach (object? arg in context.BackgroundJob.Job.Args)
            {
                if (arg is IDictionary<string, object> dict &&
                    dict.TryGetValue(IdGeneratorService.CorrelationIdKey, out object? corrId) &&
                    corrId is string correlationId &&
                    !string.IsNullOrWhiteSpace(correlationId))
                {
                    return correlationId;
                }
            }
        }

        // Try to get from current HTTP context (if job was triggered by HTTP request)
        // Use service locator pattern to resolve scoped ICorrelationIdService
        // This is safe because we're inside job execution context
        try
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            ICorrelationIdService? correlationIdService = scope.ServiceProvider.GetService<ICorrelationIdService>();
            string? httpCorrelationId = correlationIdService?.Get();

            if (!string.IsNullOrWhiteSpace(httpCorrelationId))
            {
                return httpCorrelationId;
            }
        }
        catch
        {
            // If we can't resolve ICorrelationIdService (e.g., no HTTP context), ignore and continue
        }

        // Generate new correlation ID for background jobs
        return Guid.NewGuid().ToString();
    }
}
