using RenSharp.Core;
using RenSharp.Core.ComplexParsers;
using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp
{
	internal static class DefaultConfigurationExtension
	{
		public static Configuration UseDefault(this Configuration config)
		{
			config.Skip<Label>();
			config.Skip<Goto>();
			config.Skip<Call>();
			config.Skip<Return>();

			config.Skip<Repeat>();
			config.Skip<While>();

			config.Skip<If>();
			config.Skip<Callback>();
			config.Skip<Pass>();
			config.Skip<Python>();


			config.MustPush<If>();
			config.MustPush<Repeat>();
			config.MustPush<While>();
			config.MustPush<Label>();
			config.MustPush<Init>();


			config.SetDefault("delay", "30");
			config.SetDefault("no-clear", "false");
			config.SetDefault("loop", "false");
			config.SetDefault("name", "nobody");

			return config;
		}

		public static Configuration UseCoreCommands(this Configuration config)
		{
			config.SetCommand("say", (words, config) => CommandParser.ParseMessage(words, config));
			config.SetCommand("character", (words, _) => CommandParser.ParseCharacter(words));

			config.SetCommand("init", (words, _) => CommandParser.ParseInit(words));

			config.SetCommand("else", (words, _) => CommandParser.ParseIf(words));
			config.SetCommand("if", (words, _) => CommandParser.ParseIf(words));
			config.SetCommand("callback", (words, _) => CommandParser.ParseCallback(words));

			config.SetCommand("repeat", (words, _) => CommandParser.ParseRepeat(words));
			config.SetCommand("while", (words, _) => CommandParser.ParseWhile(words));

			config.SetCommand("pass", (words, _) => CommandParser.ParsePass(words));
			config.SetCommand("", (_, __)
				=> throw new ArgumentException("Can not parse this line. Try to use 'pass' explictly."));

			config.SetCommand("label", (words, _) => CommandParser.ParseLabel(words));
			config.SetCommand("jump", (words, _) => CommandParser.ParseGoto(words));
			config.SetCommand("call", (words, _) => CommandParser.ParseCall(words));

			config.SetCommand("exit", (_, __) => CommandParser.ParseExit());
			config.SetCommand("return", (words, _) => CommandParser.ParseReturn(words));
			config.SetCommand("soft", (words, _) => CommandParser.ParseReturn(words));
			config.SetCommand("hard", (words, _) => CommandParser.ParseReturn(words));

			config.SetCommand("python", (words, _) => CommandParser.ParsePythonStart(words));
			config.SetCommand("$", (words, _) => CommandParser.ParsePythonSingle(words));

			config.AddComplex(typeof(If), (ctx, rootCmd) => IfComplexParser.Parse(ctx, rootCmd as If));
			config.AddComplex(typeof(Python), (ctx, rootCmd) => PythonComplexParser.Parse(ctx, rootCmd as Python));

			config.SetComplexPredicate<If>((x) => true);
			config.SetComplexPredicate<Python>((x) => true);

			return config;
		}
	}
}
