namespace Archetype.Infrastructure.CQRS;

public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}

public interface IVoidCommand : ICommand<Unit>
{
}
