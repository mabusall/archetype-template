namespace Archetype.Shared.Interfaces;

public abstract class BusEvent
{
    /// <summary>
    /// Gets or sets a value indicating whether the message is an outbox message.
    /// </summary>
    public bool IsOutbox { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the event was created.
    /// </summary>
    public DateTime Created { get; set; } = DateTime.UtcNow;
}