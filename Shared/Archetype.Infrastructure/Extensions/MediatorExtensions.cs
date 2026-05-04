namespace Archetype.Infrastructure.Extensions;

internal static class MediatorExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddArchetypeMediator(IReadOnlyCollection<Assembly> assemblies,
                                                                Action<MediatorConfiguration> configure)
        {
            services.AddScoped<IMediator, Mediator.Mediator>();

            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<Type> handlerTypes = assembly
                    .GetTypes()
                    .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                                t.GetInterfaces().Any(IsRequestHandlerInterface));

                foreach (Type handlerType in handlerTypes)
                {
                    IEnumerable<Type> handlers = handlerType
                        .GetInterfaces()
                        .Where(IsRequestHandlerInterface);

                    foreach (Type handlerInterface in handlers)
                    {
                        services.AddScoped(handlerInterface, handlerType);
                    }
                }
            }

            configure(new MediatorConfiguration(services));

            return services;
        }
    }

    private static bool IsRequestHandlerInterface(Type type) =>
        type.IsGenericType &&
        type.GetGenericTypeDefinition() == typeof(IRequestHandler<,>);
}