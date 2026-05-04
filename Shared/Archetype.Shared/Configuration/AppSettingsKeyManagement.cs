namespace Archetype.Shared.Configuration;

public interface IAppSettingsKeyManagement
{
    ApplicationOptions ApplicationOptions { get; }
    AzurBlobServiceOptions AzurBlobServiceOptions { get; }
    CorsConfigurationOptions CorsConfigurationOptions { get; }
    HangfireOptions HangfireOptions { get; }
    IntegrationApisOptions IntegrationApisOptions { get; }
    KeycloakOptions KeycloakOptions { get; }
    LoggingAndMonitoringOptions LoggingAndMonitoringOptions { get; }
    MobileNotificationOptions MobileNotificationOptions { get; }
    RabbitMqOptions RabbitMqOptions { get; }
    RedisOptions RedisOptions { get; }
    SmtpEmailOptions SmtpEmailOptions { get; }
    VaultSettingOptions VaultSettingOptions { get; }
}

public class AppSettingsKeyManagement(ApplicationOptions applicationOptions,
                                      AzurBlobServiceOptions azurBlobServiceOptions,
                                      CorsConfigurationOptions corsConfigurationOptions,
                                      HangfireOptions hangfireOptions,
                                      IntegrationApisOptions? integrationApisOptions,
                                      KeycloakOptions keycloakOptions,
                                      LoggingAndMonitoringOptions loggingAndMonitoringOptions,
                                      MobileNotificationOptions mobileNotificationOptions,
                                      RabbitMqOptions rabbitMqOptions,
                                      RedisOptions redisOptions,
                                      SmtpEmailOptions smtpEmailOptions,
                                      VaultSettingOptions vaultSettingOptions)
    : IAppSettingsKeyManagement
{
    public ApplicationOptions ApplicationOptions => applicationOptions;

    public AzurBlobServiceOptions AzurBlobServiceOptions => azurBlobServiceOptions;

    public CorsConfigurationOptions CorsConfigurationOptions => corsConfigurationOptions;

    public HangfireOptions HangfireOptions => hangfireOptions;

    public IntegrationApisOptions IntegrationApisOptions => integrationApisOptions!;

    public KeycloakOptions KeycloakOptions => keycloakOptions;

    public LoggingAndMonitoringOptions LoggingAndMonitoringOptions => loggingAndMonitoringOptions;

    public MobileNotificationOptions MobileNotificationOptions => mobileNotificationOptions;

    public RabbitMqOptions RabbitMqOptions => rabbitMqOptions;

    public RedisOptions RedisOptions => redisOptions;

    public SmtpEmailOptions SmtpEmailOptions => smtpEmailOptions;

    public VaultSettingOptions VaultSettingOptions => vaultSettingOptions;
}