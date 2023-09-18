using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Extensions
{
    public static class StringExtensions
    {

        public static bool ContainsCheck(this string stringToCompare, string stringToFind)
        {
            return stringToCompare.Contains(stringToFind, StringComparison.OrdinalIgnoreCase);
        }

        public static string FirstCharToUpper(this string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                return input;
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }

        public static string FirstCharToLower(this string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                return input;
            return input.First().ToString().ToLower() + String.Join("", input.Skip(1));
        }
    }
}
