using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenSharp.Core.Parse
{
    public class ParserContext
    {
        internal Configuration Config { get; set; }

        internal List<string> SourceCode = new List<string>();
        internal List<Command> Commands = new List<Command>();

        internal int Line { get; set; }
        internal int SourceLine { get; set; }
        internal string LineText => SourceCode[SourceLine - 1];
        internal bool HasNextSourceLine => SourceLine < SourceCode.Count;

        public ParserContext() { }
        public ParserContext(Configuration config)
        {
            Config = config;
        }

        internal Func<ParserContext, int, List<Command>> ParseAboveFunc { private get; set; }

		internal List<Command> ParseCommands()
		{
			Command command = ParseCommand();
			List<Command> parsed = new List<Command>() { command };

			if (Config.IsComplex(command))
				parsed.AddRange(Config.ParseComplex(this, command));

			return parsed;
		}

		internal Command ParseCommand()
		{
			string line = "";
			while (string.IsNullOrWhiteSpace(line))
			{
				SourceLine++;
				line = LineText;
			}

			int level = GetCommandLevel(line);

			line = line.Trim();
			line = ApplySyntaxSugar(line, Commands);

			string[] words = line.Split(' ');
			string keyword = words.FirstOrDefault();

			Command command = Config.CommandParsers[keyword](words, Config);

			if (command == null)
				throw new Exception($"Не возможно прочитать значение команды '{line}'");

			Line++;
			command.Line = Line;
			command.SourceLine = SourceLine;
			command.Level = level;

			return command;
		}

		internal List<Command> ParseAbove(int level)
		{
			var result = new List<Command>();

			while (true)
			{
				int previousLine = Line;
				int previousSouceLine = SourceLine;

				List<Command> parsed = ParseCommands();
				if (parsed == null || parsed.Count() <= 0)
					throw new ArgumentException($"Не получилось выполнить парсинг 'ParseAbove'. Строчка: '{previousSouceLine}'.");
				if (parsed[0].Level <= level)
				{
					Line = previousLine;
					SourceLine = previousSouceLine;
					return result;
				}
				result.AddRange(parsed);
			}
		}

		internal string NextNotEmptyLine()
        {
            while(true)
            {
                SourceLine++;
                string line = LineText;
                string trimmedLine = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                if (trimmedLine.StartsWith('#'))
                    continue;

                return line;
            }
        }
        
        internal Command SeekNext()
        {
            int line = Line;
            int sourceLine = SourceLine;
            Command next = ParseCommand();
            Line = line;
            SourceLine = sourceLine;
            return next;
        }

		internal int GetCommandLevel(string line)
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

		private string ApplySyntaxSugar(string line, List<Command> commands)
		{
			line = SyntaxSugarFormatter.ColonSugar(line);
			line = SyntaxSugarFormatter.CharacterSugar(line);
			line = SyntaxSugarFormatter.MessageSugar(line, commands);
			line = SyntaxSugarFormatter.SetSugar(line);
			line = SyntaxSugarFormatter.ElseSugar(line);
			line = SyntaxSugarFormatter.ShortenMathSugar(line);

			return line;
		}
	}
}
