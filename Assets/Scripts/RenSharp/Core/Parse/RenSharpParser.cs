using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RenSharp.Core.Parse
{
    public class RenSharpParser
    {
        
        private RenSharpValidator Validator { get; set; }
        private Configuration Config { get; set; }

        public RenSharpParser(Configuration config)
        {
            Config = config;
            Validator = new RenSharpValidator(Config);
        }

        internal List<Command> ParseCode(IEnumerable<string> codeLines)
        {
            ParserContext ctx = new ParserContext(Config);
            codeLines = codeLines.Append("exit");

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
                    throw new SyntaxErrorException($"at line {ctx.SourceLine}. Command is '{ctx.LineText}'", ex);
                }
            }

            Validator.Validate(ctx.Commands);
            return ctx.Commands;
        }

        internal static List<string> RemoveComments(List<string> code)
        {
            return code
                .Select(line =>
                {
                    int? index = RegexMethods.IndexOfComment(line);
					if (index == null)
                        return line;
                    return line.Remove(index.Value);
                })
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
