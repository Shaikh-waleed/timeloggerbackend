using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Helpers
{
    public class CryptoRandomGenerator
    {
        public static string GenerateNumericCode(int maxSize)
        {
            char[] chars = new char[62];
            chars =
                "1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GenerateAlphaNumericCode(int maxSize)
        {
            char[] chars = new char[62];
            chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GenerateRandomPassword(int minSize, int maxSize)
        {
            char[] chars = new char[62];
            chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@*$-+?_&=!%{}/".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(minSize, maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        /// <summary>
        /// Creates a pseudo-random password containing the number of character classes
        /// defined by complexity, where 2 = alpha, 3 = alpha+num, 4 = alpha+num+special.
        /// </summary>
        public static string GenerateComplexPassword(int length, int complexity = 4)
        {
            var csp = new RNGCryptoServiceProvider();
            // Define the possible character classes where complexity defines the number
            // of classes to include in the final output.
            char[][] classes =
            {
                @"abcdefghijklmnopqrstuvwxyz".ToCharArray(),
                @"ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
                @"0123456789".ToCharArray(),
                @"@*$-+?_&=!%{}/".ToCharArray(),
            };

            complexity = Math.Max(1, Math.Min(classes.Length, complexity));
            if (length < complexity)
                throw new ArgumentOutOfRangeException("length");

            // Since we are taking a random number 0-255 and modulo that by the number of
            // characters, characters that appear earilier in this array will recieve a
            // heavier weight. To counter this we will then reorder the array randomly.
            // This should prevent any specific character class from recieving a priority
            // based on it's order.
            char[] allchars = classes.Take(complexity).SelectMany(c => c).ToArray();
            byte[] bytes = new byte[allchars.Length];
            csp.GetBytes(bytes);
            for (int i = 0; i < allchars.Length; i++)
            {
                char tmp = allchars[i];
                allchars[i] = allchars[bytes[i] % allchars.Length];
                allchars[bytes[i] % allchars.Length] = tmp;
            }

            // Create the random values to select the characters
            Array.Resize(ref bytes, length);
            char[] result = new char[length];

            while (true)
            {
                csp.GetBytes(bytes);
                // Obtain the character of the class for each random byte
                for (int i = 0; i < length; i++)
                    result[i] = allchars[bytes[i] % allchars.Length];

                // Verify that it does not start or end with whitespace
                if (Char.IsWhiteSpace(result[0]) || Char.IsWhiteSpace(result[(length - 1) % length]))
                    continue;

                string testResult = new string(result);
                // Verify that all character classes are represented
                if (0 != classes.Take(complexity).Count(c => testResult.IndexOfAny(c) < 0))
                    continue;

                return testResult;
            }
        }
    }
}
