namespace Archetype.Shared.Mediator;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        where TResponse : notnull;
}
