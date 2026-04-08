namespace Archetype.Shared.DataProtection;

public interface IFileEncryptionProvider
{
    void EncryptFile(byte[] fileContent, string encryptedOutputFilePath, byte[] key, byte[] salt);

    void EncryptFile(byte[] fileContent, string encryptedOutputFilePath, string key, string salt);

    byte[] DecryptFile(string inFile, byte[] key, byte[] salt);

    byte[] DecryptFile(string inFile, string key, string salt);
}