namespace Archetype.Shared.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}