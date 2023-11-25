using RenSharp;
using RenSharp.Core.Parse;
using RenSharp.Models;
using RenSharpClient.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenSharpClient.Parser.Complex
{
	internal static class ImageComplexParser
	{
		private static List<string> AttributeAllowedNames = new List<string>()
		{
			"width",
			"height",
			"zoom"
		};

		internal static List<Command> Parse(ParserContext ctx, ImageCommand rootCmd)
		{
			// Парсим только манды которые ровно на таб больше Image
			int expectedLevel = rootCmd.Level + 1;
			Attributes attributes = rootCmd.Attributes;

			while (true)
			{
				int beforeParseLine = ctx.SourceLine;
				string lineText = ctx.NextNotEmptyLine();

				int actualLevel = ctx.GetCommandLevel(lineText);
				if (actualLevel > expectedLevel)
					throw new ArgumentException($"Неожиданный таб в команде '{lineText}'");

				string trimmedLine = lineText.Trim();

				if (trimmedLine == string.Empty || trimmedLine.Length <= 0)
					continue;

				if (actualLevel < expectedLevel)
				{
					// Stop parsing
					ctx.SourceLine = beforeParseLine;
					break;
				}

				string[] lineWords = trimmedLine.Split(' ');

				

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
