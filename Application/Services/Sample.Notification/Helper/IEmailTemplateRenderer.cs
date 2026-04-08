namespace Sample.Notification.Helper;

/// <summary>
/// Make sure you have registered razor views dependencies : services.AddRazorPages or services.AddControllersWithViews();
/// </summary>
public interface IEmailTemplateRenderer
{
    /// <typeparam name="T"></typeparam>
    /// <param name="razorViewName">viewName should match a razor page residing inside Views folder.<br/>example: "EmailsTemplates/SampleTemplate"</param>
    /// <param name="model">the model to bind razor view to</param>
    /// <returns>HTML string</returns>
    Task<string> RenderRazorTemplateAsStringAsync<T>(string razorViewName, T model);


    /// <typeparam name="T"></typeparam>
    /// <param name="razorViewPath">Absolute path of cshtml file.<br/> example: "~/EmailsTemplates/SampleTemplate.cshtml"</param>
    /// <param name="model">the model to bind razor view to</param>
    /// <returns>HTML string</returns>
    Task<string> RenderRazorTemplateFromPathAsStringAsync<T>(string razorViewPath, T model);
}