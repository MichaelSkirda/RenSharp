using RenSharp.Models;
using RenSharp.Models.Commands;
using System.Collections.Generic;

namespace RenSharp.Core.Parse.ComplexParsers
{
    internal class InitComplexParser
    {
        public static List<Command> Parse(ParserContext ctx, Init initStart)
        {
            ctx.Line++;

            // Virtual root cmd
            var python = new Python()
            {
                Line = ctx.Line,
                SourceLine = ctx.SourceLine,
                Level = initStart.Level
            };
            PythonComplexParser.Parse(ctx, python);
            
            // Как будто это просто комана 'python' внутри init
            python.Level = initStart.Level + 1;
            return new List<Command> { initStart, python };
        }
    }
}
