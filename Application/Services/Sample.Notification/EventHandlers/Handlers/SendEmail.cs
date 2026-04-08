namespace Sample.Notification.EventHandlers.Handlers;

public class SendEmailConsumer(AppSettingsKeyManagement appSettingsKeyManagement,
                               IEmailTemplateRenderer emailTemplateRenderer)
        : IConsumer<SendEmailEvent>
{
    public async Task Consume(ConsumeContext<SendEmailEvent> context)
    {
        SendEmailEvent @event = context.Message;
        SmtpEmailOptions smtpEmailOptions = appSettingsKeyManagement.SmtpEmailOptions;
        string fromAddress = smtpEmailOptions!.SenderEmail;
        MailMessage mailMessage = new()
        {
            From = new MailAddress(fromAddress, "Tasheer - فريق العمل"),
            Subject = @event.Subject,
            IsBodyHtml = true,
            Body = await RenderEmailBody(@event),
        };

        // TO Address
        if (@event.ToAddresses is not null)
        {
            foreach (MailAddress? address in @event.ToAddresses!.Select(s => new MailAddress(s)))
                mailMessage.To.Add(address);
        }

        // CC Address
        if (@event.CcAddresses is not null)
        {
            foreach (MailAddress? address in @event.CcAddresses!.Select(s => new MailAddress(s)))
                mailMessage.CC.Add(address);
        }

        // BCC Address
        if (@event.BccAddresses is not null)
        {
            foreach (MailAddress? address in @event.BccAddresses!.Select(s => new MailAddress(s)))
                mailMessage.Bcc.Add(address);
        }

        using SmtpClient smtpClient = new()
        {
            Host = smtpEmailOptions.AddressUri,
            EnableSsl = smtpEmailOptions.EnableSsl,
            Port = smtpEmailOptions.Port,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress, smtpEmailOptions.Password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = smtpEmailOptions.Timeout
        };

        // everything is ok... so far
        await smtpClient.SendMailAsync(mailMessage, context.CancellationToken);
    }

    #region [ rendering ]

    private async Task<string> RenderEmailBody(SendEmailEvent model)
    {
        string emailBody = string.Empty;
        JsonElement extraData = (JsonElement)model.ExtraData;

        switch (model.Template)
        {
            case EmailBodyTemplate.ActivateAccount:
                emailBody = await emailTemplateRenderer.
                       RenderRazorTemplateFromPathAsStringAsync($"~/Views/Account/ActivateAccount.cshtml",
                       new ActivateAccountModel
                       {
                           Name = extraData.GetPropertyValue("name"),
                           ActivationCode = extraData.GetPropertyValue("activationCode"),
                           Account = extraData.GetPropertyValue("account"),
                           Email = extraData.GetPropertyValue("email"),
                           LanguageIsoCode = model.LanguageIsoCode,
                           SiteUrl = model.SiteUrl
                       });
                break;

            case EmailBodyTemplate.ForgetPassword:
                emailBody = await emailTemplateRenderer.
                       RenderRazorTemplateFromPathAsStringAsync($"~/Views/Account/ForgetPassword.cshtml",
                       new ForgetPasswordModel
                       {
                           RedirectUrl = extraData.GetPropertyValue("redirectUrl"),
                           LanguageIsoCode = model.LanguageIsoCode
                       });
                break;

            case EmailBodyTemplate.RemoveAccount:
                emailBody = await emailTemplateRenderer.
                       RenderRazorTemplateFromPathAsStringAsync($"~/Views/Account/DeleteAccount.cshtml",
                       new DeleteAccountModel
                       {
                           Name = extraData.GetPropertyValue("name"),
                           Email = extraData.GetPropertyValue("email"),
                           LanguageIsoCode = model.LanguageIsoCode,
                           SiteUrl = model.SiteUrl
                       });
                break;
        }

        return emailBody;
    }

    #endregion
}