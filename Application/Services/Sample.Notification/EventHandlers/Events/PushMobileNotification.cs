namespace Sample.Notification.EventHandlers.Events;

public class PushMobileNotificationEvent : BusEvent
{
    public string? MessageTitleAr { get; set; }

    public string? MessageTitleEn { get; set; }

    public string? MessageBodyAr { get; set; }

    public string? MessageBodyEn { get; set; }

    public string? ImageUrl { get; set; }

    public IList<string>? DeviceTokens { get; set; }
}
