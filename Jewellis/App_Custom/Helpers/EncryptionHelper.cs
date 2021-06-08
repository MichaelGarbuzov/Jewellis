using System;
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

    }
}
