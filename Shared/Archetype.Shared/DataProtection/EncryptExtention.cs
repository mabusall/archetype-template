namespace Archetype.Shared.DataProtection;

public static class DataProtectionProviderExtention
{
    public static string PublicKey { get; private set; } = default!;

    public static string SaltKey { get; private set; } = default!;

    public static readonly char[] padding = ['='];

    public static void Initialize(string publicKey, string saltKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(publicKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(saltKey);

        PublicKey = publicKey;
        SaltKey = saltKey;
    }

    public static string Encrypt(this string textToEncrypt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(PublicKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(SaltKey);

        byte[] secretKeyByte = Encoding.UTF8.GetBytes(SaltKey);
        byte[] publicKeyByte = Encoding.UTF8.GetBytes(PublicKey);
        byte[] inputbyteArray = Encoding.UTF8.GetBytes(textToEncrypt);

        using Aes aes = Aes.Create();
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateEncryptor(publicKeyByte, secretKeyByte), CryptoStreamMode.Write);
        cs.Write(inputbyteArray, 0, inputbyteArray.Length);
        cs.FlushFinalBlock();

        string res = Convert.ToBase64String(ms.ToArray())
            .TrimEnd(padding)
            .Replace('+', '-')
            .Replace('/', '_');

        return res;
    }

    public static byte[] Encrypt(this byte[] data)
    {
        if (data is null || data.Length <= 0) return [];

        byte[] key = PublicKey.Get256ShaHash();
        byte[] salt = SaltKey.Get128MD5Hash();

        return new AesEncryptionProvider().Encrypt(data, key, salt);
    }

    public static string EncryptDB(this string textToEncrypt)
    {
        try
        {
            return textToEncrypt.Encrypt();
        }
        catch
        {
            return textToEncrypt;
        }
    }

    public static string Decrypt(this string textToDecrypt)
    {
        // Print the public and salt key to the console.
        ArgumentException.ThrowIfNullOrWhiteSpace(PublicKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(SaltKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(textToDecrypt);

        string incoming = textToDecrypt.Replace('_', '/').Replace('-', '+');
        switch (textToDecrypt.Length % 4)
        {
            case 2: incoming += "=="; break;
            case 3: incoming += "="; break;
        }

        byte[] secretKeyByte = Encoding.UTF8.GetBytes(SaltKey);
        byte[] publicKeybyte = Encoding.UTF8.GetBytes(PublicKey);
        byte[] inputbyteArray = Convert.FromBase64String(incoming);

        using Aes aes = Aes.Create();
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateDecryptor(publicKeybyte, secretKeyByte), CryptoStreamMode.Write);
        cs.Write(inputbyteArray, 0, inputbyteArray.Length);
        cs.FlushFinalBlock();

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    public static byte[] Decrypt(this byte[] data)
    {
        if (data is null || data.Length <= 0) return [];

        byte[] key = PublicKey.Get256ShaHash();
        byte[] salt = SaltKey.Get128MD5Hash();

        return new AesEncryptionProvider().Decrypt(data, key, salt);
    }

    public static string DecryptDB(this string textToDecrypt)
    {
        try
        {
            return textToDecrypt.Decrypt();
        }
        catch
        {
            return textToDecrypt;
        }
    }
}