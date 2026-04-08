namespace Archetype.Shared.DataProtection;

public static class DataProtectionUtilities
{
    /// <summary>
    /// converts string to 128 bits MD5  hash
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte[] Get128MD5Hash(this string value)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(value);

        return MD5.HashData(keyBytes);
    }

    /// <summary>
    /// converts string to 256 bits SHA hash
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte[] Get256ShaHash(this string value)
    {
        // Create a SHA256   
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));

        return bytes;
    }
}