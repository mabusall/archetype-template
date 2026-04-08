namespace Sample.Notification.EventHandlers.Events;

public class SendEmailEvent : BusEvent
{
    public string? Subject { get; set; }

    public string LanguageIsoCode { get; set; } = "en";

    public string? SiteUrl { get; set; }

    public dynamic? ExtraData { get; set; }

    public EmailNotificationAttachment? Attachment { get; set; }

    public EmailPriority Priority { get; set; }

    public EmailBodyTemplate Template { get; set; }

    public IEnumerable<string>? ToAddresses { get; set; }

    public IEnumerable<string>? CcAddresses { get; set; }

    public IEnumerable<string>? BccAddresses { get; set; }
}

public class EmailNotificationAttachment
{
    /// <summary>
    /// attachment model
    /// the data you want to use in the attachment
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// attachmant file name in email
    /// </summary>
    public string? FileName { get; set; }
}