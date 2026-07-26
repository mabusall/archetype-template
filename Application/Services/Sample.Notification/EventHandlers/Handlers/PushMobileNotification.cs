namespace Sample.Notification.EventHandlers.Handlers;

public class PushMobileNotificationConsumer : IConsumer<PushMobileNotificationEvent>
{
    public async Task Consume(ConsumeContext<PushMobileNotificationEvent> context)
    {
        PushMobileNotificationEvent @event = context.Message;
        FirebaseMessaging firebaseMessaging = FirebaseMessaging.DefaultInstance;

        Dictionary<string, string> data = new()
        {
            { "MessageTitleAr", @event.MessageTitleAr! },
            { "MessageBodyAr", @event.MessageBodyAr! },
            { "MessageTitleEn", @event.MessageTitleEn! },
            { "MessageBodyEn", @event.MessageBodyEn! },
            { "ImageUrl", @event.ImageUrl! }
        };

        if (@event.DeviceTokens is null || @event.DeviceTokens.Count == 0)
        {
            Message firebaseMessage = new()
            {
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                },
                Data = data,
                Topic = "allUsers"
            };

            await firebaseMessaging.SendAsync(firebaseMessage);
        }

        MulticastMessage firebaseMulticastMessage = new()
        {
            Android = new AndroidConfig
            {
                Priority = Priority.High,
            },
            Data = data,
            Fids = [.. @event.DeviceTokens!]
        };

        await firebaseMessaging.SendEachForMulticastAsync(firebaseMulticastMessage);
    }
}