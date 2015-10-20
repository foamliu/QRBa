using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace QRBa.Util
{
    public static class UrlHelper
    {
        private static readonly string[] code62 = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
            "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e",
            "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
            "v", "w", "x", "y", "z",  };

        public static string GetUrl(int accountId, int codeId)
        {
            uint u1 = (uint)codeId;
            uint u2 = (uint)accountId;

            ulong unsignedKey = (((ulong)u1) << 32) | u2;
            long combinedId = (long)unsignedKey;

            return string.Format("{0}i/{1}", Constants.BaseUrl, Code62Encode(combinedId));
        }

        public static string Code62Encode(long input)
        {
            long num = input;
            var sb = new StringBuilder();
            do
            {
                long k = num % 62;
                sb.Append(code62[k]);
                num = num / 62;
            }
            while (num > 0);
            char[] charArray = sb.ToString().ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static void Code62Decode(string input, out int accountId, out int codeId)
        {
            long combinedId = 0; long pow = 1;
            for (var i = input.Length - 1; i >= 0; i--)
            {
                combinedId += pow * IndexOf(input[i] + "");
                pow *= 62;
            }
            ulong unsignedKey = (ulong)combinedId;
            uint lowBits = (uint)(unsignedKey & 0xffffffffUL);
            uint highBits = (uint)(unsignedKey >> 32);
            codeId = (int)highBits;
            accountId = (int)lowBits;
        }

        private static int IndexOf(string ch)
        {
            for (var i = 0; i < 62; i++)
            {
                if (ch == code62[i])
                    return i;
            }
            return -1;
        }

    }
}