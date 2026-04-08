namespace Archetype.Shared.Exceptions;

public class PublishOutboxException : Exception
{
    public PublishOutboxException() : base("Failed to publish outbox message.") { }

}