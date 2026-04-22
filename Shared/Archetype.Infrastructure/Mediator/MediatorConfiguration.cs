namespace Archetype.Infrastructure.Mediator;

public sealed class MediatorConfiguration
{
    private readonly IServiceCollection _services;

    internal MediatorConfiguration(IServiceCollection services)
    {
        _services = services;
    }

    public MediatorConfiguration AddOpenBehavior(Type behaviorType)
    {
        _services.AddScoped(typeof(IPipelineBehavior<,>), behaviorType);
        return this;
    }
}
