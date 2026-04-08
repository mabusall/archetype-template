namespace Archetype.Shared.Interfaces;

public interface IBusService
{
    /// <summary>
    /// Publishes the specified event to the bus for processing by registered handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish. Must derive from <see cref="BusEvent"/>.</typeparam>
    /// <param name="event">The event instance to publish. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the publish operation.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : BusEvent;
}
