﻿using RenSharp.Models;
using RenSharp.Models.Commands;
using System.Collections.Generic;

namespace RenSharp.Core.Parse.ComplexParsers
{
    internal static class PythonComplexParser
    {
        internal static List<Command> Parse(ParserContext ctx, Python blockStart)
        {
            int minTab = blockStart.Level;

            while (ctx.HasNextSourceLine)
            {
                ctx.SourceLine++;
                string line = ctx.LineText;
                if (string.IsNullOrEmpty(line))
                    continue;

                int level = ctx.GetCommandLevel(line);

                if (IsValid(level, blockStart) == false)
                {
                    ctx.SourceLine--;
                    break;
                }

                blockStart.Commands.Add(line.Substring(minTab));
            }

            // Must be empty. All commands contains in blockStart
            return new List<Command>() { blockStart };
        }

        private static bool IsValid(int level, Python startBlock)
        {
            // Any cmd with lower or equals lvl is exit
            if (level <= startBlock.Level)
                return false;
            return true;
        }
    }
}
