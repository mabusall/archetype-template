namespace Archetype.Shared.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message)
        : base(message) => Details = string.Empty;

    public BadRequestException(string message, string details) : base(message)
    {
        Details = details;
    }

    public string Details { get; }
}