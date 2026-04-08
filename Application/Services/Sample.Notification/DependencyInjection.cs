
namespace Sample.Notification;

public static class DependencyInjection
{
    public static TBuilder AddNotificationModule<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // Firebase configuration - updated approach
        MobileNotificationOptions mobileNotificationOptions = builder
            .Configuration
            .GetSection(MobileNotificationOptions.ConfigurationKey)
            .Get<MobileNotificationOptions>()
            ?? throw new InvalidOperationException($"Missing configuration section '{MobileNotificationOptions.ConfigurationKey}'.");

        builder
            .Services
            .AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>()
            .AddRazorPages();

        if (mobileNotificationOptions.IsActive &&
            FirebaseApp.DefaultInstance is null)
        {
            string? jsonCredential = JsonSerializerHandler.Serialize(mobileNotificationOptions.FirebaseConfiguration);
            GoogleCredential googleCredential = CredentialFactory
                .FromJson<ServiceAccountCredential>(jsonCredential)
                .ToGoogleCredential();

            FirebaseApp.Create(new AppOptions
            {
                Credential = googleCredential
            });
        }
        return builder;
    }
}