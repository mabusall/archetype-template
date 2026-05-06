namespace Archetype.Infrastructure.Services;

public class LocalizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Get the language from Accept-Language header
        string acceptLanguage = context.Request.Headers.AcceptLanguage.ToString();

        // Split by commas to get individual language codes with optional quality values
        string? language = acceptLanguage.Split(',')
            .Select(lang => lang.Split(';')[0])  // Take the part before the semicolon (if any)
            .FirstOrDefault();

        // If the header is present but only contains "*", treat it as no specific language preference
        if (acceptLanguage.Length == 1 && acceptLanguage.Equals("*"))
            language = "en";

        if (string.IsNullOrEmpty(language))
            language = "en"; // Fallback to default language if not provided

        if (language.Contains('-'))
            language = language.Split('-')[0]; // Use only the primary language subtag

        // check what is the current language
        string currentLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower();

        if (currentLanguage != language)
        {
            // Set the current culture and UI culture for the thread
            CultureInfo cultureInfo = new(language);
            CultureInfo.CurrentCulture =
            CultureInfo.CurrentUICulture = cultureInfo;
        }

        await next(context); // Call the next middleware in the pipeline
    }
}

public static class LocalizationInterpretor
{
    public static bool IsArabic()
        => CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower().Equals("ar");

    public static string CurrentLanguage()
        => CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
}