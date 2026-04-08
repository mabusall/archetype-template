namespace Archetype.Shared.Configuration;

public static class BusConfiguration
{
    private static Action<IBusRegistrationConfigurator>? _busConfigurators;
    private static Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext>? _busFactoryConfigurators;

    public static void AddBusConfigurator(Action<IBusRegistrationConfigurator> busConfigurator)
        => _busConfigurators += busConfigurator;

    public static Action<IBusRegistrationConfigurator>? GetBusConfigurators()
        => _busConfigurators;

    public static void AddBusFactoryConfigurator(Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext> busFactoryConfigurator)
        => _busFactoryConfigurators += busFactoryConfigurator;

    public static Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext>? GetBusFactoryConfigurators()
        => _busFactoryConfigurators;
}
