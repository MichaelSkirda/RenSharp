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

        private Func<ParserContext, List<Command>> _parseFunc;
        private Func<ParserContext, Command> _parseSingle;

        internal Func<ParserContext, List<Command>> ParseFunc
        {
            set => _parseFunc = value;
        }

        internal Func<ParserContext, Command> ParseSingleFunc
        {
            set => _parseSingle = value;
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
        internal List<Command> ParseCommands() => _parseFunc(this);
        internal Command ParseSingle() => _parseSingle(this);
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
