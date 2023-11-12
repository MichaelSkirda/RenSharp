using Assets.Scripts.RenSharpClient.Models.Commands;
using RenSharp.Core.Parse;
using RenSharp.Models;
using System.Collections.Generic;

namespace Assets.Scripts.RenSharpClient.Parser.Complex
{
	internal class MenuComplexParser
	{
		internal List<Command> Parse(ParserContext ctx, Menu rootCmd)
		{
			var result = new List<Command>();
			int minimumLevel = rootCmd.Level + 1;

			string lineText = ctx.NextNotEmptyLine();

			while (true)
			{
				int lineBeforeParsing = ctx.SourceLine;

				lineText = ctx.NextNotEmptyLine();

				int actualLevel = RenSharpParser.GetCommandLevel(lineText);

				if(actualLevel < minimumLevel)
				{
					ctx.SourceLine = lineBeforeParsing;
				}
			}
		}
	}
}
