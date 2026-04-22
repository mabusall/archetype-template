namespace Archetype.Infrastructure.Mediator;

internal sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        where TResponse : notnull
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = serviceProvider.GetRequiredService(handlerType);
        var behaviors = serviceProvider
            .GetServices(typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType))
            .OfType<object>()
            .Reverse();

        RequestHandlerDelegate<TResponse> pipeline = ct =>
        {
            MethodInfo handleMethod = handlerType.GetMethod(nameof(IRequestHandler<,>.Handle))!;
            return (Task<TResponse>)handleMethod.Invoke(handler, [request, ct])!;
        };

        foreach (object behavior in behaviors)
        {
            RequestHandlerDelegate<TResponse> next = pipeline;
            MethodInfo handleMethod = behavior.GetType().GetMethod(nameof(IPipelineBehavior<,>.Handle))!;
            pipeline = ct => (Task<TResponse>)handleMethod.Invoke(behavior, [request, next, ct])!;
        }

        return await pipeline(cancellationToken);
    }
}
