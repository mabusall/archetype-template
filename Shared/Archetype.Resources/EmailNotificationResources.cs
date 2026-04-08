namespace Archetype.Resources;

[DebuggerNonUserCode()]
[System.Runtime.CompilerServices.CompilerGenerated()]
public class EmailNotificationResources
{
    private static System.Resources.ResourceManager? resourceMan;
    private static CultureInfo? resourceCulture;

    internal EmailNotificationResources()
    {

    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public static System.Resources.ResourceManager ResourceManager
    {
        get
        {
            if (ReferenceEquals(resourceMan, null))
            {
                System.Resources.ResourceManager temp = new("Tasheer.Resources.EmailNotificationResources", typeof(EmailNotificationResources).Assembly);
                resourceMan = temp;
            }
            return resourceMan;
        }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
        get
        {
            return resourceCulture!;
        }
        set
        {
            resourceCulture = value;
        }
    }

    public static string Team
        => ResourceManager.GetString("Team", resourceCulture)!;
    public static string DoNotReplyToThisMail
        => ResourceManager.GetString("DoNotReplyToThisMail", resourceCulture)!;
    public static string Copyright
        => ResourceManager.GetString("Copyright", resourceCulture)!;
    public static string WelcomeMessage
        => ResourceManager.GetString("WelcomeMessage", resourceCulture)!;
    public static string NeedHelp
        => ResourceManager.GetString("NeedHelp", resourceCulture)!;
    public static string NeedForHelpMessage
        => ResourceManager.GetString("NeedForHelpMessage", resourceCulture)!;

    public static string ActivateAccountTemplate_Title
        => ResourceManager.GetString("ActivateAccountTemplate_Title", resourceCulture)!;
    public static string ActivateAccountTemplate_Body
        => ResourceManager.GetString("ActivateAccountTemplate_Body", resourceCulture)!;

    public static string ForgetPasswordTemplate_Title
        => ResourceManager.GetString("ForgetPasswordTemplate_Title", resourceCulture)!;
    public static string ForgetPasswordTemplate_Body
        => ResourceManager.GetString("ForgetPasswordTemplate_Body", resourceCulture)!;

    public static string RemoveAccountTemplate_Title
        => ResourceManager.GetString("RemoveAccountTemplate_Title", resourceCulture)!;
    public static string RemoveAccountTemplate_Body
        => ResourceManager.GetString("RemoveAccountTemplate_Body", resourceCulture)!;
}
