namespace Archetype.Shared.DataProtection;

public class AesFileEncryptionProvider(IEncryptionProvider encryptionProvider) : IFileEncryptionProvider
{
    /// <summary>
    /// if there is a file at encryptedOutputFilePath, then it will be overritten
    /// </summary>
    /// <param name="fileContent"></param>
    /// <param name="encryptedOutputFilePath"></param>
    /// <param name="key"></param>
    public void EncryptFile(byte[] fileContent,
                            string encryptedOutputFilePath,
                            byte[] key,
                            byte[] salt)
    {
        byte[] data = encryptionProvider.Encrypt(fileContent, key, salt);
        File.WriteAllBytes(encryptedOutputFilePath, data);
    }

    public void EncryptFile(byte[] fileContent,
                            string encryptedOutputFilePath,
                            string key,
                            string salt)
    {
        EncryptFile(fileContent, encryptedOutputFilePath, key.Get256ShaHash(), salt.Get128MD5Hash());
    }

    public byte[] DecryptFile(string inFile, byte[] key, byte[] salt)
        => encryptionProvider.Decrypt(File.ReadAllBytes(inFile), key, salt);

    public byte[] DecryptFile(string inFile, string key, string salt)
        => DecryptFile(inFile, key.Get256ShaHash(), salt.Get128MD5Hash());
}