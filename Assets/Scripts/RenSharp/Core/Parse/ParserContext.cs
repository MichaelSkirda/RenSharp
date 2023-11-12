using RenSharp.Models;
using System;
using System.Collections.Generic;

namespace RenSharp.Core.Parse
{
    public class ParserContext
    {
        internal List<string> SourceCode = new List<string>();
        internal List<Command> Commands = new List<Command>();

        internal int Line { get; set; }
        internal int SourceLine { get; set; }
        internal string LineText => SourceCode[SourceLine - 1];
        internal bool HasNextSourceLine => SourceLine < SourceCode.Count;

        internal Func<ParserContext, List<Command>> ParseFunc { private get; set; }
		internal Func<ParserContext, Command> ParseSingleFunc { private get; set; }
        internal Func<ParserContext, int, List<Command>> ParseAboveFunc { private get; set; }

		internal List<Command> ParseCommands() => ParseFunc(this);
		internal Command ParseSingle() => ParseSingleFunc(this);
        internal List<Command> ParseAbove(int level) => ParseAboveFunc(this, level);

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
            Command next = ParseSingle();
            Line = line;
            SourceLine = sourceLine;
            return next;
        }
    }
}
