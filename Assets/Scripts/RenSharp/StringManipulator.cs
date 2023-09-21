using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RenSharp
{
    public static class StringManipulator
    {
        public static string GetStringBetween(this string str, string value) => GetStringBetween(str, value, value);
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

        public static List<string> GetVars(string expression)
        {
            string clearedExpression = Regex.Replace(expression, "(=|>|<|\\+|-|\\*|\\/|\\%|true|false)", "");

			while (true)
			{
                // Delete constant strings "like this"
				if (clearedExpression.Contains("\"") == false)
					break;
				string substring = $"\"{clearedExpression.GetStringBetween("\"")}\"";
				clearedExpression = clearedExpression.Replace(substring, "");
			}

			List<string> vars = clearedExpression
				.Split(" ")
				.Where(x => string.IsNullOrEmpty(x) == false)
                .Where(x => Int32.TryParse(x, out _) == false)
				.ToList();

            return vars;
		}

        public static bool IsNumber(this string str)
        {
            return Int32.TryParse(str, out _);
        }
    }
}
