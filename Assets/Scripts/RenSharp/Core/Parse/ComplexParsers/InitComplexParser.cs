using RenSharp.Core.Parse;
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
            var python = new Python()
            {
                Line = ctx.Line,
                SourceLine = ctx.SourceLine,
                Level = initStart.Level // parse like same level with initStart 
            };
            PythonComplexParser.Parse(ctx, python);
            // But actually it's level bigger
            python.Level = initStart.Level + 1;
            return new List<Command> { python };
        }
    }
}
