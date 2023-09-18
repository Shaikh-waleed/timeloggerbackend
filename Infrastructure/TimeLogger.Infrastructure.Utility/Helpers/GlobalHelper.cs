using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Helpers
{
    public static class GlobalHelper
    {
        public static string NewCode(string lastCode, int length)
        {
            var lastAddedCode = System.Numerics.BigInteger.Parse(lastCode);
            lastAddedCode++;
            var newCode = lastAddedCode.ToString();
            if (newCode.Length < length)
                while (newCode.Length < length)
                    newCode = "0" + newCode;

            return newCode;
        }
    }
}
