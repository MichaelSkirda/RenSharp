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
            Commands = Config.CommandParsers;
        }

        internal List<Command> ParseCode(List<string> codeLines)
        {
			ReaderContext ctx = new ReaderContext();

            ctx.ParseFunc = ParseCommands;
            ctx.ParseSingleFunc = ParseCommand;
            ctx.SourceCode = codeLines;
			ctx.SourceCode = RemoveComments(ctx.SourceCode);
            RemoveNullOrEmptyFromEnd(ctx.SourceCode);

            while (ctx.SourceLine < ctx.SourceCode.Count)
            {
                try
                {
                    List<Command> parsed = ctx.ParseCommands();
                    foreach(Command command in parsed)
                    {
						Validate(command, ctx.Commands.LastOrDefault());
						ctx.Commands.Add(command);
					}
				}
				catch (Exception ex)
                {
                    throw new Exception($"at line {ctx.SourceLine}. Commans is '{ctx.LineText}'", ex);
                }
            }

            return ctx.Commands;
        }

		internal List<Command> ParseCommands(ReaderContext ctx)
        {
			Command command = ParseCommand(ctx);

            if(Config.IsComplex(command))
            {
                List<Command> parsed = new List<Command>() { command };
                Type type = command.GetType();
                parsed.AddRange(Config.ComplexCommandParsers[type](ctx, command));
                return parsed;
            }

            return new List<Command>() { command };
        }

        private Command ParseCommand(ReaderContext ctx)
        {
			string line = "";
            while(string.IsNullOrWhiteSpace(line))
            {
				ctx.SourceLine++;
                line = ctx.LineText;
			}

			int level = GetCommandLevel(line);

			line = line.Trim();
			line = ApplySyntaxSugar(line, ctx.Commands);

			string[] words = line.Split(' ');
			string keyword = words.FirstOrDefault();

			Command command = Commands[keyword](words, Config);

			if (command == null)
				throw new Exception($"Cannot parse command '{line}'");

            ctx.Line++;
			command.Line = ctx.Line;
			command.Level = level;

			return command;
		}

        private static string ApplySyntaxSugar(string line, List<Command> commands)
        {
            line = SyntaxSugarFormatter.CharacterSugar(line);
            line = SyntaxSugarFormatter.MessageSugar(line, commands);
            line = SyntaxSugarFormatter.SetSugar(line);
            line = SyntaxSugarFormatter.ElseSugar(line);
            line = SyntaxSugarFormatter.ShortenMathSugar(line);

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

        internal static int GetCommandLevel(string line)
        {
            int level = 1;
            foreach (char chr in line)
            {
                if (chr != '\t')
                    break;
				level++;
			}
			return level;
        }
        internal static List<string> RemoveComments(List<string> code)
        {
            return code
                .Select(x => x.DeleteAfter("//"))
                .ToList();
        }

		private void RemoveNullOrEmptyFromEnd(List<string> sourceCode)
		{
            while(string.IsNullOrWhiteSpace(sourceCode.Last()))
            {
                sourceCode.RemoveAt(sourceCode.Count - 1);
            }
		}
	}
}
