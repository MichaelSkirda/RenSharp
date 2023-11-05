using Assets.Scripts.RenSharpClient.Models.Commands;
using Microsoft.Scripting.Actions;
using RenSharp;
using RenSharp.Core;
using RenSharp.Core.Parse;
using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.RenSharpClient.Parser.Complex
{
	internal static class ImageComplexParser
	{
		private static List<string> AttributeAllowedNames = new List<string>()
		{
			"width",
			"height",
			"zoom"
		};

		internal static List<Command> Parse(ParserContext ctx, Image rootCmd)
		{
			// Парсим только манды которые ровно на таб больше Image
			int expectedTab = rootCmd.Level + 1;
			Attributes attributes = rootCmd.Attributes;

			while (true)
			{
				int beforeParseLine = ctx.SourceLine;

				ctx.SourceLine++;
				string lineText = ctx.LineText;

				int actualTab = RenSharpParser.GetCommandLevel(lineText);
				if (actualTab > expectedTab)
					throw new ArgumentException($"Неожиданный таб в команде '{lineText}'");

				string trimmedLine = lineText.Trim();

				if (trimmedLine == string.Empty || trimmedLine.Length <= 0)
					continue;

				string[] lineWords = trimmedLine.Split(' ');

				if (actualTab < expectedTab)
				{
					// Stop parsing
					ctx.SourceLine = beforeParseLine;
					break;
				}

				string name = lineWords[0];
				bool unallowedName = !AttributeAllowedNames.Contains(name);

				if (unallowedName)
					throw new ArgumentException($"Неизвестное имя атрибута '{name}'.");

				bool alreadyExists = attributes.ContainsKey(name);
				if (alreadyExists)
					throw new ArgumentException($"Атрибут '{name}' объявлен два или более раза у команды 'image'.");

				string value = lineWords.Skip(1).ToWord();
				attributes.AddAttribute(name, value, rewrite: true);
			}

			return new List<Command>() { rootCmd };
		}
	}
}
