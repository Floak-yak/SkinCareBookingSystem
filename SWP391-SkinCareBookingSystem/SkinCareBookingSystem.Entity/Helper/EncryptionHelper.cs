using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Helper
{
    public static class EncryptionHelper
    {
        // NEVER hardcode these in real apps! Store in a secure place like Azure Key Vault or environment variables.
        private static readonly string EncryptionKey = "Xp5+Y3g+TzlpK0vtslTqWz2R8dgdqIqYFb2cY7tHcO8="; // Must be 32 bytes for AES-256
        private static readonly string IV = "gX/0eH5i4z2Ch4uTeyXjPA=="; // Must be 16 bytes for AES

        public static byte[] Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(EncryptionKey);
                aes.IV = Convert.FromBase64String(IV);
                aes.Mode = CipherMode.CBC;

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return encryptedBytes;
                }
            }
        }

        public static string Decrypt(byte[] encryptedBytes)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(EncryptionKey);
                aes.IV = Convert.FromBase64String(IV);
                aes.Mode = CipherMode.CBC;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}
