using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RenSharp.Core
{
    public class RenSharpReader
    {
        private Dictionary<string, Func<string[], Configuration, Command>> Commands
            = new Dictionary<string, Func<string[], Configuration, Command>>();

        private Configuration Config { get; set; }

        public RenSharpReader(Configuration config)
        {
            Config = config;
            Commands = Config.Commands;
        }

        internal List<Command> ParseCode(List<string> code)
        {
            RemoveComments(code);
            List<Command> commands = new List<Command>();
            RenSharpContext context = new RenSharpContext();

            int line = 1;
            for (int i = 0; i < code.Count; i++)
            {
                string codeLine = code[i];
                if (NotCommand(codeLine))
                    continue;

                try
                {
                    List<Command> parsed = ParseCommand(codeLine, commands);
                    foreach(Command command in parsed)
                    {
						Validate(command, context, codeLine);
						context.Level = command.Level;
						command.Line = line;
						line++;
						commands.AddRange(parsed);
					}
                }
                catch (Exception ex)
                {
                    throw new Exception($"at line {line}", ex);
                }
            }

            return commands;
        }

        internal List<Command> ParseCommand(string line, List<Command> commands)
        {
            int level = GetCommandLevel(line);
            line = line.Trim();
			line = ApplySyntaxSugar(line, commands);

            string[] words = line.Split(' ');
            string keyword = words.FirstOrDefault();

            Command command = Commands[keyword](words, Config);

            if (command == null)
                throw new Exception($"Cannot parse command '{line}'");

            command.Level = level;

            return new List<Command>() { command };
        }

        private static string ApplySyntaxSugar(string line, List<Command> commands)
        {
            line = SyntaxSugarFormatter.CharacterSugar(line);
            line = SyntaxSugarFormatter.MessageSugar(line, commands);
            line = SyntaxSugarFormatter.SetSugar(line);

            return line;
		}

		private static void Validate(Command command, RenSharpContext context, string codeLine)
        {
            if (command.Level <= 0)
                throw new Exception($"Command '{codeLine}' not valid. Tabulation can not be less than zero.");

            if (command.Level >= context.Level + 2)
                throw new Exception($"Command '{codeLine}' not valid. Tabulation can not be higher by two then previous.");
        }

        internal static int GetCommandLevel(string command)
        {
            int level = 0;
            foreach (char chr in command)
            {
                level++;
                if (chr != '\t')
                    break;
            }
            return level;
        }
        internal static void RemoveComments(List<string> code) => code.ForEach(x => x = x.DeleteAfter("//"));
        private static bool NotCommand(string str) => string.IsNullOrEmpty(str.Trim());
    }
}
