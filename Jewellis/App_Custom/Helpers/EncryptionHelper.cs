using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Jewellis.App_Custom.Helpers
{
    /// <summary>
    /// Represents a helper class for encryption.
    /// </summary>
    public static class EncryptionHelper
    {

        /// <summary>
        /// Hashes the specified text using the SHA256 algorithm.
        /// </summary>
        /// <param name="text">The text to hash.</param>
        /// <returns>Returns the hash string.</returns>
        public static string HashSHA256(string text)
        {
            // SHA512 is disposable by inheritance.  
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Generates a random salt for hashing.
        /// </summary>
        /// <returns>Returns the generated random salt.</returns>
        public static string GenerateSalt()
        {
            byte[] bytes = new byte[128 / 8];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Encrypts a text via AES encryption method.
        /// </summary>
        /// <param name="clearText">The text to encrypt.</param>
        /// <param name="key">The key for the encryption.</param>
        /// <returns>Returns the cipher text.</returns>
        public static string EncryptAES(string clearText, string key)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        /// <summary>
        /// Decrypts a text via AES decryption method.
        /// </summary>
        /// <param name="cipherText">The encrypted text to decrypt.</param>
        /// <param name="key">The key for the encryption.</param>
        /// <returns>Returns the text decrypted.</returns>
        public static string DecryptAES(string cipherText, string key)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}
