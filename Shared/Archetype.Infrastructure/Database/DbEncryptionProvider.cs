namespace Archetype.Infrastructure.Database;

public class DbEncryptionProvider : ValueConverter<string, string>
{
    public DbEncryptionProvider()
        : base(e => e.EncryptDB(), e => e.DecryptDB())
    {
    }
}

public class DbBinaryEncryptionProvider : ValueConverter<byte[], byte[]>
{
    public DbBinaryEncryptionProvider()
        : base(e => e.Encrypt(), e => e.Decrypt())
    {
    }
}