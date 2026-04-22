namespace Archetype.Shared.Mediator;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);

public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
