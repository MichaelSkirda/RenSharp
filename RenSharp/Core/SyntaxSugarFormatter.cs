using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

			return $"set {line}";
		}

		private static bool IsCharacter(List<Command> commands, string name)
		{
			IEnumerable<Character> characters = commands.OfType<Character>();
			return characters.Any(x => x.Name == name);
		}
	}
}
