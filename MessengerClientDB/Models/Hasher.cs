using System;
using System.Security.Cryptography;

namespace MessengerClientDB.Models
{
    public class Hasher
    {
        public const int SALT_SIZE = 32;
        public const int HASH_SIZE = 32;
        public const int ITERATION_COUNT = 50000;

        // Given a password, it generates the hash & returns it along with its salt & iteration count.
        public static string HashGenerator(string password)
        {
            string salt = SaltGenerator();
            string hash = HashHelper(password, salt, ITERATION_COUNT);
            return hash + ":" + salt + ":" + ITERATION_COUNT;
        }

        // Reproduces the hash by using the stored salt, the stored iteration count, & the
        // password the user entered to login.
        public static bool RightPassword(string storedInfo, string providedPassword)
        {
            string[] values = storedInfo.Split(':');
            string providedHash = HashHelper(providedPassword, values[1], Convert.ToInt32(values[2]));
            return CompareHashes(values[0], providedHash);
        }

        // Produces a hash value given a password, salt, & iteration count.
        private static string HashHelper(string password, string salt, int iterations)
        {
            byte[] slt = Convert.FromBase64String(salt);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, slt, iterations);
            byte[] hashedPwd = pbkdf2.GetBytes(HASH_SIZE);
            return Convert.ToBase64String(hashedPwd);
        }

        // Returns salt. 
        private static string SaltGenerator()
        {
            RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_SIZE];
            generator.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        // Must compare every character every time to prevent "timing attacks." E.g, It might take only a 
        // millisecond to determine that the first hashed character is incorrect, but ten milliseconds to find
        // a failure farther into the text, and a hacker could monitor these timing variations. 
        private static bool CompareHashes(string original, string provided)
        {
            uint differences = (uint)original.Length ^ (uint)provided.Length;
            for (int position = 0; position < Math.Min(original.Length, provided.Length); position++)
            {
                differences |= (uint)(original[position] ^ provided[position]);
            }
            return (differences == 0);
        }

        /*
        // Reproduces the hash by using the stored salt, the stored iteration count, & the
        // password the user entered to login.
        public static string ReproduceHash(string providedPassword, string salt, int iterationCount)
        {
            string hash = HashHelper(providedPassword, salt, iterationCount);
            return hash + ":" + salt + ":" + iterationCount;
        }

        */
    }
}
