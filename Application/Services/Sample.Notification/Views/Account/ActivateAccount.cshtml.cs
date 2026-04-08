namespace Sample.Notification.Views.Account;

public class ActivateAccountModel : PageModel
{
    [BindProperty]
    public string? Name { get; set; }

    [BindProperty]
    public string? Account { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public string? ActivationCode { get; set; }

    [BindProperty]
    public string? SiteUrl { get; set; }

    [BindProperty]
    public string? LanguageIsoCode { get; set; }
}
