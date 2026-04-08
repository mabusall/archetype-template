namespace Archetype.Shared.Configuration;

public class BasicAuthenticationOptions
{
    public static string ConfigurationKey => "BasicAuthentication";

    public required string UserName { get; set; }
    public required string Password { get; set; }
}