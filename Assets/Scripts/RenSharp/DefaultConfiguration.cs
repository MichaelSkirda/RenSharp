using RenSharp.Core;
using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp
{
	internal static class DefaultConfigurationExtension
	{
		public static void UseDefault(this Configuration config)
		{
			config.Skip<Label>();
			config.Skip<Goto>();
			config.Skip<If>();
			config.Skip<Set>();
			config.Skip<Repeat>();
			config.Skip<While>();
			config.Skip<Callback>();
			config.Skip<Pass>();

			config.AllowToPushStack<If>();
			config.AllowToPushStack<Repeat>();
			config.AllowToPushStack<While>();

			config.SetDefault("delay", "30");
			config.SetDefault("no-clear", "false");
			config.SetDefault("loop", "false");
			config.SetDefault("name", "nobody");
		}

		public static void UseCoreCommands(this Configuration config)
		{
			config.SetCommand("say", (words, config) => CommandParser.ParseMessage(words, config));
			config.SetCommand("character", (words, _) => CommandParser.ParseCharacter(words));

			config.SetCommand("label", (words, _) => CommandParser.ParseLabel(words));
			config.SetCommand("goto", (words, _) => CommandParser.ParseGoto(words));

			config.SetCommand("set", (words, _) => CommandParser.ParseSet(words));
			config.SetCommand("else", (words, _) => CommandParser.ParseIf(words));
			config.SetCommand("if", (words, _) => CommandParser.ParseIf(words));
			config.SetCommand("callback", (words, _) => CommandParser.ParseCallback(words));

			config.SetCommand("repeat", (words, _) => CommandParser.ParseRepeat(words));
			config.SetCommand("while", (words, _) => CommandParser.ParseWhile(words));

			config.SetCommand("pass", (words, _) => CommandParser.ParsePass(words));
			config.SetCommand("", (_, __)
				=> throw new ArgumentException("Can not parse this line. Try to use 'pass' explictly."));

			config.AddComplex(typeof(If), (ctx, rootCmd) => ComplexParser.ComplexIfParser(ctx, rootCmd as If));
		}
	}
}
