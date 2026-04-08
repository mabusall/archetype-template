namespace Archetype.Shared.DataProtection;

public interface IEncryptionProvider
{
    byte[] Encrypt(byte[] content, byte[] key, byte[] salt);
    byte[] Encrypt(byte[] content, string key, string salt) => Encrypt(content, key.Get256ShaHash(), salt.Get128MD5Hash());

    byte[] Decrypt(byte[] encryptedContent, byte[] key, byte[] salt);
    byte[] Decrypt(byte[] encryptedContent, string key, string salt) => Decrypt(encryptedContent, key.Get256ShaHash(), salt.Get128MD5Hash());

    string EncryptText(string content, byte[] key, byte[] salt) => Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(content), key, salt));
    string DecryptText(string encryptedContent, byte[] key, byte[] salt) => Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(encryptedContent), key, salt));
}