using Assets.Scripts.RenSharpClient.Models.Commands;
using RenSharp.Core.Parse;
using RenSharp.Models;
using RenSharp.Models.Commands;
using RenSharp.RenSharpClient.Models;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.RenSharpClient.Parser.Complex
{
	internal class MenuComplexParser
	{
		// Create anonymous label for each button.
		private static int AnonymousLabelId = 0;

		private string NextLabelName()
		{
			AnonymousLabelId++;
			return "_rs_anonymous_" + AnonymousLabelId;
		}

		internal List<Command> Parse(ParserContext ctx, Menu rootCmd)
		{
			var result = new List<Command>() { rootCmd };
			int expectedLevel = rootCmd.Level + 1;

			int lineBeforeParsing = ctx.SourceLine;
			string lineText = GetNextMenuLine(ctx, expectedLevel);

			if(lineText.EndsWith(":") == false)
			{
				string[] words = lineText.Split(' ');
				try
				{
					Message command = CommandParser.ParseMessage(words, ctx.Config);
					result.Add(command);
				}
				catch (Exception ex)
				{
					throw new ArgumentException("В первой команде блока menu без символа ':' в конце может идти только команда 'say'.", ex);
				}
			}
			else
			{
				ctx.SourceLine = lineBeforeParsing;
			}

			while (true)
			{
				try
				{
					lineText = GetNextMenuLine(ctx, expectedLevel);
				}
				catch
				{
					break;
				}
				int level = ctx.GetCommandLevel(lineText);
				
				MenuButton button = ParseButton(lineText);
				rootCmd.Buttons.Add(button);

				string anonymousName = NextLabelName();
				Label anonymousLabel = new Label(anonymousName);
				anonymousLabel.SetLine(ctx);
				anonymousLabel.Level = level;

				List<Command> commands = ctx.ParseAbove(level);

				result.Add(anonymousLabel);
				result.AddRange(commands);
			}

			if (rootCmd.Buttons.Count <= 0)
				throw new ArgumentException("Блок 'menu' обязан содержать как минимум 1 кнопку.");
			return result;
		}

		private MenuButton ParseButton(string line)
		{
			line = line.Trim();
			if (line.StartsWith("\"") == false || line.EndsWith(":") == false)
				throw new ArgumentException("Каждая кнопка блока 'menu' должна начинаться с символа '\"' и заканчиваться символом ':''");

		}

		private string GetNextMenuLine(ParserContext ctx, int expectedLevel)
		{
			int lineBeforeParsing = ctx.SourceLine;
			try
			{
				string lineText = ctx.NextNotEmptyLine();
				int actualLevel = RenSharpParser.GetCommandLevel(lineText);
				AssertLevel(actualLevel, expectedLevel);
				return lineText;
			}
			catch
			{
				ctx.SourceLine = lineBeforeParsing;
				throw;
			}
		}

		private void AssertLevel(int actualLevel, int expectedLevel)
		{
			if (actualLevel < expectedLevel)
				throw new ArgumentException("Блок 'menu' не может быть пустым.");
			else if (actualLevel > expectedLevel)
				throw new ArgumentException("Неожиданный tab в начале блока 'menu'.");
		}

	}
}
