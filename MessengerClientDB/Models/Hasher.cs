using System;
using System.Security.Cryptography;

namespace MessengerClientDB.Models
{
    public class Hasher
    {
        public const int SALT_SIZE = 32;
        public const int HASH_SIZE = 32;
        public const int ITERATION_COUNT = 50000;

        // Generates a hash given the password & the salt. Returns the hashed password. 
        public static string HashGenerator(string password)
        {
            string hash, hashSaltIter;
            string salt = SaltGenerator();
            hash = HashHelper(password, salt, ITERATION_COUNT);
            hashSaltIter = hash + ":" + salt + ":" + ITERATION_COUNT;
            return hashSaltIter;
        }

        // Generates a hash given the password & the salt. Returns the hashed password. 
        public static string ReproduceHash(string password, string salt, int iterationCount)
        {
            string hash, hashSaltIter;
            hash = HashHelper(password, salt, iterationCount);
            hashSaltIter = hash + ":" + salt + ":" + iterationCount;
            return hashSaltIter;
        }

        public static bool CheckHash(string hashSaltIter, string providedPassword)
        {
            string hashToCompare;
            string[] values = hashSaltIter.Split(':');
            hashToCompare = HashHelper(providedPassword, values[1], Convert.ToInt32(values[2]));
            return CompareHashes(values[0], hashToCompare);
        }

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

    }
}
