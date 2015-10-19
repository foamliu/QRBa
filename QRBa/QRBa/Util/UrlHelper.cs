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

        public static string GetUrl(int adId)
        {
            return string.Format("{0}{1}", Constants.BaseUrl, Code62Encode(adId));
        }

        public static string Code62Encode(int input)
        {
            int num = input;
            var sb = new StringBuilder();
            do
            {
                int k = num % 62;
                sb.Append(code62[k]);
                num = num / 62;
            }
            while (num > 0);
            char[] charArray = sb.ToString().ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static int Code62Decode(string input)
        {
            int result = 0; int pow = 1;
            for (var i = input.Length - 1; i >= 0; i--)
            {
                result += pow * IndexOf(input[i] + "");
                pow *= 62;
            }
            return result;
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