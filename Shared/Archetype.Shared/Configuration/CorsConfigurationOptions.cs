namespace Archetype.Shared.Configuration;

public class CorsConfigurationOptions
{
    public static string ConfigurationKey => "Cors";

    /// <summary>
    /// Enable or disable CORS globally
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Policy name to use
    /// </summary>
    public required string PolicyName { get; set; }

    /// <summary>
    /// Allow any origin (use only in development!)
    /// </summary>
    public bool AllowAnyOrigin { get; set; } = false;

    /// <summary>
    /// List of allowed origins (e.g., ["https://app1.example.com", "https://app2.example.com"])
    /// </summary>
    public string[] AllowedOrigins { get; set; } = [];

    /// <summary>
    /// Allow credentials (cookies, authorization headers, etc.)
    /// </summary>
    public bool AllowCredentials { get; set; } = true;

    /// <summary>
    /// Specific headers to allow (empty = allow any)
    /// </summary>
    public string[] AllowedHeaders { get; set; } = [];

    /// <summary>
    /// Specific methods to allow (empty = allow any)
    /// </summary>
    public string[] AllowedMethods { get; set; } = [];

    /// <summary>
    /// Headers that can be exposed to the browser
    /// </summary>
    public string[] ExposedHeaders { get; set; } = [];

    /// <summary>
    /// How long (in seconds) the browser should cache the preflight response
    /// </summary>
    public int PreflightMaxAgeSeconds { get; set; } = 600; // 10 minutes
}
