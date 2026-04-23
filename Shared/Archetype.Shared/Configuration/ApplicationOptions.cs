namespace Archetype.Shared.Configuration;

public enum DatabaseProviderType
{
    SqlServer = 1,
    PostgreSql,

    //[Obsolete("To be supported in future")]
    //MySql,

    //[Obsolete("To be supported in future")]
    //Sqlite,

    //[Obsolete("To be supported in future")]
    //Oracle
}

public class ApplicationOptions
{
    public static string ConfigurationKey => "Application";

    public required string ApiKey { get; set; }
    public required string PrivateKey { get; set; }
    public required string PublicKey { get; set; }
    public string[] SupportedLanguages { get; set; } = [];
    public required string DbConnection { get; set; }
    public required DatabaseProviderType DbProviderType { get; set; }
}