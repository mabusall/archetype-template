namespace Sample.Notification.Views.Account;

public class DeleteAccountModel : PageModel
{
    [BindProperty]
    public string? Name { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public string? SiteUrl { get; set; }

    [BindProperty]
    public string? LanguageIsoCode { get; set; }
}
