namespace Archetype.Shared.Configuration;

public record AzurBlobServiceOptions
{
    public static string ConfigurationKey => "AzurBlobService";

    public required string AddressUri { get; set; }
    public required string CdnAddressUri { get; set; }
    public required string Token { get; set; }
}
