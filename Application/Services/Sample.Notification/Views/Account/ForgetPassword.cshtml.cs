namespace Sample.Notification.Views.Account;

public class ForgetPasswordModel : PageModel
{
    [BindProperty]
    public string? RedirectUrl { get; set; }

    [BindProperty]
    public string? LanguageIsoCode { get; set; }

    [BindProperty]
    public string? SiteUrl { get; set; }
}
