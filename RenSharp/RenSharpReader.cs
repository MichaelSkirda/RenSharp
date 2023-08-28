using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RenSharp
{
	internal class RenSharpReader
	{
		internal static List<Command> ParseCode(string path)
		{
			var lines = File.ReadAllLines(path).ToList();
			return ParseCode(lines);
		}

		internal static List<Command> ParseCode(List<string> code)
		{
			RemoveComments(code);
			List<Command> commands = new List<Command>();
			RenSharpContext context = new RenSharpContext();

			int line = 1;
			for(int i = 0; i < code.Count; i++)
			{
				string codeLine = code[i];
				if (NoCommand(codeLine))
					continue;
				line++;

				try
				{
					Command command = ParseCommand(codeLine);
					Validate(command, context, codeLine);
					context.Level = command.Level;
					commands.Add(command);
				}
				catch(Exception ex)
				{
					throw new Exception($" at line {i}", ex);
				}
			}

			return commands;
		}

		internal static Command ParseCommand(string line)
		{
			string[] words = line.Trim().Split(' ');
			string keyword = "";
			string argument = "";
			List<string> args = new List<string>();

			try
			{
				keyword = words[0];
				argument = words[1];
				args = words.Skip(1).ToList();
			}
			catch
			{

			}

			Command command = null;

			if(keyword == "label")
			{
				command = new Label(argument);
			}
			else if(keyword == "character")
			{
				command = new Character(argument, args);
			}

			if (command == null)
				throw new Exception($"Cannot parse command '{line}'");

			command.Level = GetCommandLevel(line);

			return command;
		}

		private static void Validate(Command command, RenSharpContext context, string codeLine)
		{
			if (command.Level <= 0)
				throw new Exception($"Command '{codeLine}' not valid! Tabulation can not be less than zero!");

			if (command.Level >= (context.Level + 2))
				throw new Exception($"Command '{codeLine}' not valid! Tabulation can not be higher by two then previous!");
		}

		internal static int GetCommandLevel(string command)
		{
			int level = 0;
			foreach(char chr in command)
			{
				level++;
				if (chr != '\t')
					break;
			}
			return level;
		}

		internal static void RemoveComments(List<string> code)
		{
			code.ForEach(x => x = x.DeleteAfter("//"));
		}

		private static bool NoCommand(string str)
		{
			return String.IsNullOrEmpty(str.Trim());
		}

	}
}
