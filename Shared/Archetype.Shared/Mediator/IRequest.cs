namespace Archetype.Shared.Mediator;

public interface IRequest<out TResponse>
    where TResponse : notnull
{
}
