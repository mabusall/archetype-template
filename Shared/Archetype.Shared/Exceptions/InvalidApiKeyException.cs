namespace Archetype.Shared.Exceptions;

public class InvalidApiKeyException : Exception
{
    public InvalidApiKeyException(string message) : base(message)
    {
    }

    public InvalidApiKeyException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
