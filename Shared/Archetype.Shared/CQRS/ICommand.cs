namespace Archetype.Shared.CQRS;

public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}

public interface IVoidCommand : ICommand<Unit>
{
}
