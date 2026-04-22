namespace Archetype.Shared.Interfaces.CQRS;

public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}

public interface IVoidCommand : ICommand<Unit>
{
}
