using RenSharp.Models.Parse;
using System;
using System.Text.RegularExpressions;

namespace RenSharp.Core.Parse
{
    /// <summary>
    /// Не применяется на Python код.
    /// </summary>
    internal static class SyntaxSugarFormatter
    {
        internal static string CharacterSugar(string line)
        {
            // If no character - character is nobody
            if (line.StartsWith("\""))
                line = "say _rs_nobody_char " + line;

            return line;
        }

        internal static string MessageSugar(string line, Configuration config)
        {
			string firstWord = line.Split(' ')[0];
            if (config.IsKeyword(firstWord))
                return line;

            StringFirstQuotes quotedString = CommandParser.BetweenQuotesFirst(line);
            string name = quotedString.Before;

            if (string.IsNullOrWhiteSpace(name) || quotedString.Between == null)
                return line;
            if(RegexMethods.IsValidCharacterName(name))
                return "say " + line;
            return line;
		}

		internal static string ColonSugar(string line)
        {
            // Colons at end are optional
            if (line.EndsWith(":"))
                return line.Substring(0, line.Length - 1);
            return line;
        }

        internal static string SetSugar(string line)
        {
            // x = 123 -> ['x ', ' 123']
            // y=42 -> ['y', '42']
            string[] keyValue = line.Split("=");
            if (keyValue.Length != 2)
                return line;

            string key = keyValue[0].Trim();
            string value = keyValue[1].Trim();

            if (key.Contains(" "))
                return line;

            return $"$ {line}";
        }

        internal static string ShortenMathSugar(string line)
        {
            string[] words = line.Split(' ');

            // set x++
            // set y--
            if (words.Length == 1)
            {
                bool parsed = TryParseIncrement(words[0], out string result);
                if (parsed)
                    return result;
                else
                    return line;
            }



            return line;
        }

        internal static string SpaceAfterIfNotNeccessary(string line)
        {
            if (line.StartsWith("if("))
                return "if (" + line.Substring(3);
            return line;
        }

        internal static string SpaceAfterWhileNotNeccessary(string line)
        {
			if (line.StartsWith("while("))
				return "while (" + line.Substring(6);
			return line;
		}


		private static bool TryParseIncrement(string expression, out string result)
        {
            string name = expression.Replace("--", "").Replace("++", "");

            var increment = new Regex("^[a-zA-Z]+\\+\\+$");
            var decrement = new Regex("^[a-zA-Z]+--$");

            var incMatches = increment.Matches(expression).Count;
            var decMatches = decrement.Matches(expression).Count;

            if (incMatches > 1 || decMatches > 1)
                throw new ArgumentException("Not allowed to use more than one '++' or '--' at same line.");

            if (incMatches > 0 && decMatches > 0)
                throw new ArgumentException("Not allowed to use '++' and '--' at same line.");

            if (incMatches > 0)
            {
                result = $"$ {name} = {name} + 1";
                return true;
            }
            else if (decMatches > 0)
            {
                result = $"$ {name} = {name} - 1";
                return true;
            }
            else
            {
                result = "";
                return false;
            }
        }


        internal static string ElseSugar(string line)
        {
            if (line.Trim() == "else")
                return "else if True";
            return line;
        }
    }
}
