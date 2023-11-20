using RenSharp.Core.Parse;
using RenSharp.Models;
using RenSharp.Models.Commands;
using RenSharp.Models.Parse;
using RenSharp.RenSharpClient.Models;
using RenSharpClient.Models.Commands;
using System;
using System.Collections.Generic;

namespace RenSharpClient.Parser.Complex
{
	internal static class MenuComplexParser
	{
		// Create anonymous label for each button.
		private static int AnonymousLabelId = 0;

		private static string NextLabelName()
		{
			AnonymousLabelId++;
			return "_rs_anonymous_" + AnonymousLabelId;
		}

		internal static List<Command> Parse(ParserContext ctx, Menu rootCmd)
		{
			var result = new List<Command>() { rootCmd };
			int expectedLevel = rootCmd.Level + 1;

			string afterMenuAnonymousName = NextLabelName();

			int sourceLineBeforeParsing = ctx.SourceLine;
			int lineBeforeParsing = ctx.Line;
			string menuItemLine = GetNextMenuLine(ctx, expectedLevel);

			if(menuItemLine.EndsWith(":") == false)
			{
				string[] words = menuItemLine.Split(' ');
				try
				{
					Message message = CommandParser.ParseMessage(words, ctx.Config);
					rootCmd.Message = message;
				}
				catch (Exception ex)
				{
					ctx.Line = lineBeforeParsing;
					ctx.SourceLine = sourceLineBeforeParsing;
					throw new ArgumentException("В первой команде блока menu без символа ':' в конце может идти только команда 'say'.", ex);
				}
			}
			else
			{
				ctx.SourceLine = sourceLineBeforeParsing;
			}

			while (true)
			{
				try
				{
					menuItemLine = GetNextMenuLine(ctx, expectedLevel);
				}
				catch
				{
					break;
				}
				int level = ctx.GetCommandLevel(menuItemLine);

				string anonymousName = NextLabelName();
				MenuButton button = ParseButton(menuItemLine, anonymousName);
				rootCmd.Buttons.Add(button);

				Label anonymousLabel = new Label(anonymousName);
				// Go to end of menu after button block commands
				Jump jump = new Jump(afterMenuAnonymousName, evaluate: false);
				anonymousLabel.SetLine(ctx);
				anonymousLabel.Level = level;
				jump.SetLine(ctx);
				jump.Level = level + 1;

				List<Command> commands = ctx.ParseAbove(level);

				result.Add(anonymousLabel);
				result.AddRange(commands);
				result.Add(jump);
			}

			Label afterMenuAnonymousLabel = new Label(afterMenuAnonymousName);
			Pass pass = new Pass();

			afterMenuAnonymousLabel.SetLine(ctx);
			pass.SetLine(ctx);

			afterMenuAnonymousLabel.Level = rootCmd.Level;
			pass.Level = rootCmd.Level + 1;

			result.Add(afterMenuAnonymousLabel);
			result.Add(pass);

			if (rootCmd.Buttons.Count <= 0)
				throw new ArgumentException("Блок 'menu' обязан содержать как минимум 1 кнопку.");
			return result;
		}

		private static MenuButton ParseButton(string line, string label)
		{
			line = line.Trim();
			if (line.StartsWith("\"") == false || line.EndsWith(":") == false)
				throw new ArgumentException("Каждая кнопка блока 'menu' должна начинаться с символа '\"' и заканчиваться символом ':''");

			StringFirstQuotes quotes = CommandParser.BetweenQuotesFirst(line);
			string buttonText = quotes.Between;
			string afterQuotes = quotes.After.Trim();
			if (afterQuotes == ":")
				return new MenuButton(buttonText, label);
			else if (afterQuotes.StartsWith("if"))
			{
				// Delete if keyword save only predicate
				// Delete colon ':' at end
				afterQuotes = afterQuotes.Substring(2);
				afterQuotes = afterQuotes.Substring(0, afterQuotes.Length - 1);
				return new MenuButton(buttonText, label, afterQuotes);
			}

			throw new ArgumentException($"Не получается пропасить кнопку меню {line}");
		}

		private static string GetNextMenuLine(ParserContext ctx, int expectedLevel)
		{
			int lineBeforeParsing = ctx.SourceLine;
			try
			{
				string lineText = ctx.NextNotEmptyLine();
				int actualLevel = ctx.GetCommandLevel(lineText);
				AssertLevel(actualLevel, expectedLevel);
				return lineText;
			}
			catch
			{
				ctx.SourceLine = lineBeforeParsing;
				throw;
			}
		}

		private static void AssertLevel(int actualLevel, int expectedLevel)
		{
			if (actualLevel < expectedLevel)
				throw new ArgumentException("Блок 'menu' не может быть пустым.");
			else if (actualLevel > expectedLevel)
				throw new ArgumentException("Неожиданный tab в начале блока 'menu'.");
		}

	}
}
