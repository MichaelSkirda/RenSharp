using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp
{
    public static class StringManipulator
    {
        public static string GetStringBetween(this string str, string left, string right)
        {
            int start = str.IndexOf(left) + left.Length;
            int end = str.IndexOf(right, start);

            if (start == -1 || end == -1)
                return "";

            string result = str.Substring(start, end - start);

            return result;
        }

		public static string DeleteAfter(this string str, string value)
        {
            int index = str.IndexOf(value);

            if (index != -1)
                return str.Substring(0, index);

            return str;
        }
    }
}
