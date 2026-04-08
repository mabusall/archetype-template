namespace Archetype.Shared.Configuration;

public class SmtpEmailOptions
{
    public static string ConfigurationKey => "SmtpEmail";

    public required string AddressUri { get; set; }
    public int Port { get; set; }
    public required string Password { get; set; }
    public int Timeout { get; set; }
    public bool EnableSsl { get; set; }
    public required string SenderEmail { get; set; }
}
