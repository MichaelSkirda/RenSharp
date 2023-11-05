using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RenSharp.Core.Parse
{
    public class RenSharpParser
    {
        private Dictionary<string, Func<string[], Configuration, Command>> Parsers { get; set; }
            = new Dictionary<string, Func<string[], Configuration, Command>>();
        
        private RenSharpValidator Validator { get; set; }
        private Configuration Config { get; set; }

        public RenSharpParser(Configuration config)
        {
            Config = config;
            Parsers = Config.CommandParsers;
            Validator = new RenSharpValidator(Config);
        }

        internal List<Command> ParseCode(IEnumerable<string> codeLines)
        {
            ParserContext ctx = new ParserContext();
            codeLines = codeLines.Append("exit");

            ctx.ParseFunc = ParseCommands;
            ctx.ParseSingleFunc = ParseCommand;
            ctx.SourceCode = codeLines.ToList();
            ctx.SourceCode = RemoveComments(ctx.SourceCode);
            // Обязательно. Когда complex parser читает он не проверяет есть ли следующая строчка
            RemoveNullOrEmptyFromEnd(ctx.SourceCode);

            while (ctx.HasNextSourceLine)
            {
                try
                {
                    List<Command> parsed = ctx.ParseCommands();
                    foreach (Command command in parsed)
                    {
                        Validator.Validate(command, ctx.Commands.LastOrDefault());
                        ctx.Commands.Add(command);
                    }
                }
                catch (Exception ex)
                {
                    throw new SyntaxErrorException($"at line {ctx.SourceLine}. Commans is '{ctx.LineText}'", ex);
                }
            }

            Validator.Validate(ctx.Commands);
            return ctx.Commands;
        }

        internal List<Command> ParseCommands(ParserContext ctx)
        {
            Command command = ParseCommand(ctx);
            List<Command> parsed = new List<Command>() { command };

            if (Config.IsComplex(command))
                parsed.AddRange(Config.ParseComplex(ctx, command));

            return parsed;
        }

        private Command ParseCommand(ParserContext ctx)
        {
            string line = "";
            while (string.IsNullOrWhiteSpace(line))
            {
                ctx.SourceLine++;
                line = ctx.LineText;
            }

            int level = GetCommandLevel(line);

            line = line.Trim();
            line = ApplySyntaxSugar(line, ctx.Commands);

            string[] words = line.Split(' ');
            string keyword = words.FirstOrDefault();

            Command command = Parsers[keyword](words, Config);

            if (command == null)
                throw new Exception($"Не возможно прочитать значение команды '{line}'");

            ctx.Line++;
            command.Line = ctx.Line;
            command.SourceLine = ctx.SourceLine;
            command.Level = level;

            return command;
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
                .Select(x => x.DeleteAfter("#"))
                .ToList();
        }

        private void RemoveNullOrEmptyFromEnd(List<string> sourceCode)
        {
            while (string.IsNullOrWhiteSpace(sourceCode.Last()))
            {
                sourceCode.RemoveAt(sourceCode.Count - 1);
            }
        }
    }
}
