using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace RenSharp.Core
{
	internal static class SyntaxSugarFormatter
	{
		internal static string CharacterSugar(string line)
		{
			// If no character - character is nobody
			if (line.StartsWith("\""))
				line = "say nobody " + line;

			return line;
		}

		internal static string MessageSugar(string line, List<Command> commands)
		{
			// If no say command specified but character name given
			string keyword = line.Split(' ')[0];
			if (IsCharacter(commands, keyword))
				line = "say " + line;

			return line;
		}

		internal static string ColonSugar(string line)
		{
			if(line.EndsWith(":"))
				return line.Substring(0, line.Length - 1);
			return line;
		}

		internal static string CallbackSugar(string line)
		{
			//TODO
			return line;
			/*List<string> methodNames = CallbackAttribute.Callbacks.Keys
				.Select(x => x)
				.ToList();

			// If line starts with any method name
			if(methodNames.Any(x => line.StartsWith(x)))
				return "callback " + line;
			return line; */
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

			if (line.StartsWith("$"))
				line = line.Remove(0, 1).Trim();

			return $"set {line}";
		}

		internal static string ShortenMathSugar(string line)
        {
			string[] words = line.Split(' ');

			// set x++
			// set y--
			if(words.Length == 1)
			{
				bool parsed = TryParseIncrement(words[0], out string result);
				if (parsed)
					return result;
				else
					return line;
			}



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

			if(incMatches > 0)
			{
				result = $"set {name} = {name} + 1";
				return true;
			}
			else if(decMatches > 0)
			{
				result = $"set {name} = {name} - 1";
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
				return "else if true";
			return line;
		}

		private static bool IsCharacter(List<Command> commands, string name)
		{
			IEnumerable<Character> characters = commands.OfType<Character>();
			return characters.Any(x => x.Name == name);
		}
	}
}
