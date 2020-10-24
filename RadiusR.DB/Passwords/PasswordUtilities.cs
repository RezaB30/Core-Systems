using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Passwords
{
    public static class PasswordUtilities
    {
        /// <summary>
        /// Creates a 6 disgit password for RADIUS.
        /// </summary>
        /// <returns></returns>
        public static string GenerateInternetPassword()
        {
            Random rnd = new Random();
            string result = "";
            for (int i = 0; i < 6; i++)
            {
                result += rnd.Next(0, 10).ToString();
            }
            return result;
        }

        /// <summary>
        /// Generates a secure password.
        /// </summary>
        /// <param name="length">The length of the password generated.</param>
        /// <returns></returns>
        public static string GenerateSecurePassword(int length)
        {
            var characterPalette = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&*.-+/?";
            var rand = new Random();
            var results = string.Empty;
            for (int i = 0; i < length; i++)
            {
                results += characterPalette[rand.Next(characterPalette.Length)];
            }
            return results;
        }

        /// <summary>
        /// Hashes a string in SHA256 hex (CAPS).
        /// </summary>
        /// <param name="plainPassword">Plain password text.</param>
        /// <returns></returns>
        public static string HashPassword(string plainPassword)
        {
            SHA256 encoded = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(plainPassword);
            var hashed = encoded.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashed.Length; i++)
            {
                builder.Append(hashed[i].ToString("X2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Hashes a string in SHA1 hex (lower case).
        /// </summary>
        /// <param name="plainPassword">Plain password text.</param>
        /// <returns></returns>
        public static string HashLowSecurityPassword(string plainPassword)
        {
            var algorithm = SHA1.Create();
            var calculatedHash = string.Join("", algorithm.ComputeHash(Encoding.UTF8.GetBytes(plainPassword)).Select(b => b.ToString("x2")));
            return calculatedHash;
        }
    }
}
