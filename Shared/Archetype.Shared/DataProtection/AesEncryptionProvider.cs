namespace Archetype.Shared.DataProtection;

public class AesEncryptionProvider : IAesEncryptionProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="key">up to 256 bits</param>
    /// <param name="salt">up to 128 bits</param>
    /// <returns></returns>
    public byte[] Encrypt(byte[] content, byte[] key, byte[] salt)
    {
        // Create instance of Aes for symmetric encryption of the data.
        Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = salt;

        ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);

        byte[] response;
        using (MemoryStream outFs = new())
        {
            // Now write the cipher text using a CryptoStream for encrypting.
            using (CryptoStream outStreamEncrypted = new(outFs, transform, CryptoStreamMode.Write))
            {
                // By encrypting a chunk at a time, you can save memory and accommodate large files.
                int count = 0;
                int offset = 0;

                // blockSizeBytes can be any arbitrary size.
                int blockSizeBytes = aes.BlockSize / 8;
                byte[] data = new byte[blockSizeBytes];
                int bytesRead = 0;

                using (MemoryStream inStream = new(content, true))
                {
                    do
                    {
                        count = inStream.Read(data, 0, blockSizeBytes);
                        offset += count;
                        outStreamEncrypted.Write(data, 0, count);
                        bytesRead += blockSizeBytes;
                    }
                    while (count > 0);
                }

                outStreamEncrypted.FlushFinalBlock();
                outStreamEncrypted.Close();
            }
            response = outFs.ToArray();
            outFs.Close();
        }

        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="encryptedContent"></param>
    /// <param name="key">up to 256 bits</param>
    /// <param name="salt">up to 128 bits</param>
    /// <returns></returns>
    public byte[] Decrypt(byte[] encryptedContent, byte[] key, byte[] salt)
    {
        // Create instance of Aes for symetric decryption of the data.
        Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = salt;

        byte[] result;

        // Use MemoryStream to read the encrypted input stream and return decrypted content.
        using (MemoryStream inStream = new(encryptedContent))
        {
            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream outStream = new();

            int count = 0;
            int offset = 0;

            // blockSizeBytes can be any arbitrary size.
            int blockSizeBytes = aes.BlockSize / 8;
            byte[] data = new byte[blockSizeBytes];

            // Start at the beginning  of the cipher text.
            inStream.Seek(0, SeekOrigin.Begin);

            try
            {
                using CryptoStream outStreamDecrypted = new(outStream, transform, CryptoStreamMode.Write);
                do
                {
                    count = inStream.Read(data, 0, blockSizeBytes);
                    offset += count;
                    outStreamDecrypted.Write(data, 0, count);
                }
                while (count > 0);

                outStreamDecrypted.FlushFinalBlock();
                outStreamDecrypted.Close();
            }
            catch
            {
                //fail gracefully   
            }
            finally
            {
                result = outStream.ToArray();
                outStream.Close();
            }
        }

        return result;
    }
}