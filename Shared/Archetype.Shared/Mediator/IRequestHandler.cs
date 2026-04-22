namespace Archetype.Shared.Mediator;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}
