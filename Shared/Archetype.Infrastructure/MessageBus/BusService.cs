
namespace Archetype.Infrastructure.MessageBus;

public class BusService(IBus bus,
                        IPublishEndpoint outbox,
                        OutBoxDbContext dbContext,
                        ILogger<BusService> logger,
                        ICorrelationIdService correlationIdService)
    : IBusService
{
    public async Task Publish<TEvent>(TEvent @event,
                                      CancellationToken cancellationToken)
        where TEvent : BusEvent
    {
        // Get current correlation ID from the request context
        string correlationId = correlationIdService.Get();

        if (@event.IsOutbox)
        {
            try
            {
                // Publish with correlation ID in headers for end-to-end traceability
                await outbox.Publish(@event, context =>
                {
                    // Propagate correlation ID through message headers
                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        context.Headers.Set(IdGeneratorService.CorrelationIdKey, correlationId);
                    }
                }, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError(ex, "Error while publishing outbox event {Event}", @event);
                }
                throw new PublishOutboxException();
            }
        }
        else
        {
            // Publish with correlation ID in headers for end-to-end traceability
            await bus.Publish(@event, context =>
            {
                // Propagate correlation ID through message headers
                if (!string.IsNullOrEmpty(correlationId))
                {
                    context.Headers.Set(IdGeneratorService.CorrelationIdKey, correlationId);
                }
            }, cancellationToken);
        }
    }
}