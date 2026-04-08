namespace Archetype.Infrastructure.MessageBus;

/// <summary>
/// MassTransit consume filter that logs all consumer activity (similar to MediatR LoggingBehavior).
/// Automatically applied to all consumers for centralized logging.
/// </summary>
public class LoggingConsumeFilter<T>(ILogger<LoggingConsumeFilter<T>> logger) : IFilter<ConsumeContext<T>>
    where T : class
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        string messageType = typeof(T).Name;
        string consumerId = context.ReceiveContext.InputAddress?.AbsolutePath ?? "unknown";

        // Get correlation ID from message headers or generate new one
        string correlationId = context.CorrelationId?.ToString()
            ?? context.Headers.Get<string>(IdGeneratorService.CorrelationIdKey)
            ?? Guid.NewGuid().ToString();

        // Get user information if available
        string userId = context.Headers.Get<string>("user.id") ?? string.Empty;

        // Add correlation ID to logging scope (makes it searchable in logs as labels.SystemCorrelationId)
        // This is CRITICAL for structured logging and log correlation
        using IDisposable? logScope = logger.BeginScope(new Dictionary<string, object?>
        {
            [IdGeneratorService.CorrelationIdKey] = correlationId
        });

        // Create a span for this consumer
        using Activity? activity = Activity.Current?.Source.StartActivity($"Consumer {messageType}", ActivityKind.Consumer);

        // Enrich activity with consumer context
        if (activity is not null)
        {
            activity.SetTag("messaging.system", "RabbitMQ");
            activity.SetTag("messaging.operation", "consume");
            activity.SetTag("messaging.destination", consumerId);
            activity.SetTag("messaging.message_type", messageType);
            activity.SetTag("messaging.conversation_id", context.ConversationId?.ToString() ?? string.Empty);
            activity.SetTag(IdGeneratorService.CorrelationIdKey, correlationId);

            if (!string.IsNullOrEmpty(userId))
            {
                activity.SetTag("user.id", userId);
            }

            // Set Elastic APM span label
            Elastic.Apm.Agent.Tracer.CurrentSpan?.SetLabel(IdGeneratorService.CorrelationIdKey, correlationId);
            Elastic.Apm.Agent.Tracer.CurrentSpan?.SetLabel("messaging.message_type", messageType);
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "[START] Consuming {MessageType} from {ConsumerId}",
                messageType,
                consumerId);

            logger.LogInformation(
                "Message Context: ConversationId={ConversationId}, MessageId={MessageId}, Message={@Message}",
                context.ConversationId,
                context.MessageId,
                context.Message);
        }

        Stopwatch sw = Stopwatch.StartNew();

        try
        {
            // Call the next filter or consumer
            await next.Send(context);

            sw.Stop();
            double elapsedSeconds = sw.Elapsed.TotalSeconds;

            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("consumer.duration_seconds", elapsedSeconds);

            if (elapsedSeconds > 3 && logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning(
                    "[PERFORMANCE] {MessageType} consumption took {ElapsedSeconds:F2}s",
                    messageType,
                    elapsedSeconds);
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "[END] {MessageType} consumed successfully in {ElapsedSeconds:F2}s",
                    messageType,
                    elapsedSeconds);
            }
        }
        catch (Exception ex)
        {
            sw.Stop();

            if (activity is not null)
            {
                activity.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity.AddException(ex);
            }

            logger.LogError(ex,
                "[ERROR] Failed to consume {MessageType} after {ElapsedSeconds:F2}s. Exception: {ExceptionMessage}",
                messageType,
                sw.Elapsed.TotalSeconds,
                ex.Message);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("loggingConsumeFilter");
    }
}
