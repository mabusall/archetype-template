namespace Archetype.Shared.Configuration;

public class RabbitMqOptions
{
    public static string ConfigurationKey => "MessageBus";

    public required string AddressUri { get; set; }
    public required string AmqpAddressUri { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Prefix { get; set; }
    public required string License { get; set; }
    public bool IsEnabled { get; set; }
    public bool AutoStart { get; set; }
    public bool AutoDelete { get; set; }
    public bool DisableDeliveryService { get; set; }
    public bool DisableInboxCleanupService { get; set; }
    public bool LicenseAvailable { get; set; }
    public Dictionary<string, ConsumerEndpoint> Consumers { get; set; } = [];
}

public class ConsumerEndpoint
{
    public bool IsActive { get; set; }
    public required string Consumer { get; set; }
    public bool UseDatabase { get; set; }
    public int PrefetchCount { get; set; }
    public int ConcurrentMessageLimit { get; set; }
    public int RetryCount { get; set; }
    public TimeSpan Interval { get; set; }
}