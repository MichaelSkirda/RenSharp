using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenSharp.Core.ComplexParsers
{
	internal static class PythonComplexParser
	{
		internal static List<Command> Parse(ReaderContext ctx, Python blockStart)
		{
			int minTab = blockStart.Level;

			while (ctx.HasNextSourceLine)
			{
				ctx.SourceLine++;
				string line = ctx.LineText;
				int level = RenSharpReader.GetCommandLevel(line);

				if (IsValid(level, blockStart) == false)
				{
					ctx.SourceLine--;
					break;
				}

				blockStart.Commands.Add(line.Substring(minTab));
			}



			// Must be empty. All commands contains in blockStart
			return new List<Command>();
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
