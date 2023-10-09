using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
			//[a-zA-Z{1}][a-zA-Z0-9_]+(?=[\-+*/%,) ]|$) <- old. Uses positive lookahead.
			// words that have no '('
			var regex = new Regex("\\b[a-zA-Z]{1}[a-zA-Z0-9_]*\\b(?!\\()");
            return regex.Matches(expression)
                .Select(x => x.Value)
                .Distinct()
                .ToList();
		}

        public static List<string> GetMethods(string expression)
        {
			var regex = new Regex("([a-zA-Z{1}][a-zA-Z0-9_]+)(?=\\()");
            return regex.Matches(expression)
                .Select(x => x.Value)
                .Distinct()
                .ToList();
        }

        public static bool IsNumber(this string str)
        {
            return Int32.TryParse(str, out _);
        }

		internal static ExpressionMembers GetMembers(string expression)
		{
			// Delete all math operators, true/false, const numbers (42, 12, 10) and const string ("hello, world", "foo")
			string clearedExpression = Regex.Replace
				(expression, "(=|>|<|\\+|-|\\*|\\/|\\%|\\b(true|false|[0-9]+)\\b|\".*?\")", " ");

			return new ExpressionMembers()
            {
                Methods = GetMethods(clearedExpression),
                Variables = GetVars(clearedExpression)
            };
		}

        internal static string ToPythonCode(this IEnumerable<string> str)
            => string.Join(Environment.NewLine, str);

		public static string ToWord(this IEnumerable<string> words) => string.Join(" ", words);
	}
}
