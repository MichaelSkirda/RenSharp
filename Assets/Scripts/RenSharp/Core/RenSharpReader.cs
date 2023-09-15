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

        internal List<Command> ParseCode(List<string> codeLines)
        {
			ReaderContext ctx = new ReaderContext();

			ctx.SourceCode = codeLines;
			ctx.SourceCode = RemoveComments(ctx.SourceCode);

            while (ctx.Line < ctx.SourceCode.Count)
            {
                try
                {
                    List<Command> parsed = ParseCommands(ctx);
                    foreach(Command command in parsed)
                    {
						Validate(command, ctx.Commands.LastOrDefault());
						ctx.Commands.Add(command);
					}
				}
				catch (Exception ex)
                {
                    throw new Exception($"at line {ctx.Line}. Commans is '{ctx.LineText}'", ex);
                }
            }

            return ctx.Commands;
        }

        internal List<Command> ParseCommands(ReaderContext ctx)
        {
			Command command = ParseCommand(ctx);

            if(command is If)
            {
				List<Command> parsed = new List<Command>() { command };
				parsed.AddRange(ParseIf(ctx, command as If));
                return parsed;
            }

            return new List<Command>() { command };
        }

        private Command ParseCommand(ReaderContext ctx)
        {
            ctx.Line++;
			int level = GetCommandLevel(ctx);

			string line = ctx.LineText;
			line = line.Trim();
			line = ApplySyntaxSugar(line, ctx.Commands);

			string[] words = line.Split(' ');
			string keyword = words.FirstOrDefault();

			Command command = Commands[keyword](words, Config);

			if (command == null)
				throw new Exception($"Cannot parse command '{line}'");

			command.Line = ctx.Line;
			command.Level = level;

			return command;
		}

		private List<Command> ParseIf(ReaderContext ctx, If rootIf)
        {
            List<Command> commands = new List<Command>();
            int endIfLine;
            while(true)
            {
                Command command = ParseCommand(ctx);
                if (command.IsNot<Nop>() && command.IsNot<If>() && command.Level <= rootIf.Level)
                {
                    endIfLine = command.Line;
                    ctx.Line--;
					break;
				}
                if(command.Is<If>() && (command as If).IsRoot)
                {
                    endIfLine = command.Line;
					ctx.Line--;
					break;
                }
				commands.Add(command);
			}
			commands
                .OfType<If>()
                .ToList()
                .ForEach(x => x.EndIfLine = endIfLine);
            rootIf.EndIfLine = endIfLine;
            return commands;
        }

        private static string ApplySyntaxSugar(string line, List<Command> commands)
        {
            line = SyntaxSugarFormatter.CharacterSugar(line);
            line = SyntaxSugarFormatter.MessageSugar(line, commands);
            line = SyntaxSugarFormatter.SetSugar(line);
            line = SyntaxSugarFormatter.ElseSugar(line);

            return line;
		}

		private void Validate(Command command, Command previousCmd)
        {
            if (command.Level <= 0)
                throw new Exception($"Command '{command.GetType()}' not valid. Tabulation can not be less than zero.");

            if (previousCmd == null)
                return;

            if(Config.CanPush(previousCmd) == false)
            {
                if (previousCmd.Level < command.Level)
                    throw new Exception($"Command '{previousCmd.GetType()}' can not push tabulation.");
            }

            if (command.Level >= previousCmd.Level + 2)
                throw new Exception($"Command '{command.GetType()} not valid. Tabulation can not be higher by two then previous.");
        }

        internal static int GetCommandLevel(ReaderContext ctx)
        {
            string command = ctx.LineText;
            if (string.IsNullOrWhiteSpace(command))
                return ctx.Commands.Last().Level;

            int level = 0;
            foreach (char chr in command)
            {
                level++;
                if (chr != '\t')
                    break;
            }
            return level;
        }
        internal static List<string> RemoveComments(List<string> code)
        {
            return code
                .Select(x => x.DeleteAfter("//"))
                .ToList();
        }

        internal static List<string> RemoveNullOrEmpty(List<string> code)
            => code.Where(x => String.IsNullOrEmpty(x.Trim()) == false).ToList();
        private static bool NullOrEmpty(string str) => string.IsNullOrEmpty(str.Trim());
    }
}
