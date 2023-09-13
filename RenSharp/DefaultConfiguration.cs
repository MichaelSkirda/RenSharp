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

			config.SetDefault("delay", "30");
			config.SetDefault("no-clear", "false");
			config.SetDefault("loop", "false");
			config.SetDefault("name", "nobody");
		}

		public static void UseCoreCommands(this Configuration config)
		{
			config.AddCommand("say", (words, config) => CommandParser.ParseMessage(words, config));
			config.AddCommand("character", (words, config) => CommandParser.ParseCharacter(words));
			config.AddCommand("label", (words, config) => CommandParser.ParseLabel(words));
			config.AddCommand("goto", (words, config) => CommandParser.ParseGoto(words));
			config.AddCommand("load", (words, config) => CommandParser.ParseLoad(words));
			config.AddCommand("set", (words, config) => CommandParser.ParseSet(words));
			config.AddCommand("if", (words, config) => CommandParser.ParseIf(words));
		}
	}
}
