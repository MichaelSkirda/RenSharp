using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using RenSharp.Core.Parse;
using RenSharp.Core.Parse.ComplexParsers;
using RenSharp.Models.Commands;
using RenSharp.Models.Commands.Json;
using RenSharpClient.Models.Commands.Json;
using System;
using System.Linq;

namespace RenSharp
{
    public static class DefaultConfiguration
	{
		public static Configuration GetDefaultConfig()
			=> new Configuration().UseDefault().UseCoreCommands().UseJsonParsers();

		public static Configuration UseDefault(this Configuration config)
		{
			config.Skip<Label>();
			config.Skip<Jump>();
			config.Skip<Call>();
			config.Skip<Return>();

			config.Skip<Repeat>();
			config.Skip<While>();

			config.Skip<If>();
			config.Skip<Callback>();
			config.Skip<Pass>();
			config.Skip<Python>();
			config.Skip<Define>();
			config.Skip<Init>();

			config.Skip<SysSetScope>();


			config.MustPush<If>();
			config.MustPush<Repeat>();
			config.MustPush<While>();
			config.MustPush<Label>();
			config.MustPush<Init>();


			config.SetDefault("delay", "30");
			config.SetDefault("no-clear", "false");
			config.SetDefault("loop", "false");
			config.SetDefault("name", "_rs_nobody_name");

			return config;
		}

		public static Configuration UseCoreCommands(this Configuration config)
		{
			config.SetCommand("say", (words, config) => CommandParser.ParseMessage(words, config));

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
			config.SetCommand("define", (words, _) => CommandParser.ParseDefine(words));
			config.SetCommand("$", (words, _) => CommandParser.ParsePythonSingle(words)); // One line python

			config.AddComplex(typeof(If), (ctx, rootCmd) => IfComplexParser.Parse(ctx, rootCmd as If));
			config.AddComplex(typeof(Python), (ctx, rootCmd) => PythonComplexParser.Parse(ctx, rootCmd as Python));
			config.AddComplex(typeof(Init), (ctx, rootCmd) => InitComplexParser.Parse(ctx, rootCmd as Init));

			config.SetComplexPredicate<If>(x => true);
			config.SetComplexPredicate<Python>(x => true);
			config.SetComplexPredicate<Init>(x => (x as Init).IsPython);

			return config;
		}

		public static Configuration UseJsonParsers(this Configuration config)
		{
            config.DeserializeParsers.Add("syssetscope", (json, core) =>
            {
				SysSetScopeJson commandParsed = JsonConvert.DeserializeObject<SysSetScopeJson>(json);
				ScriptScope scope = core.Context.PyEvaluator.CreateScope(commandParsed.variables);
				var command = new SysSetScope(scope);
				command.SetPosition(commandParsed);
                return command;
            });

            config.DeserializeParsers.Add("messagerollback", (json, core) =>
            {
                MessageRollbackJson commandParsed = JsonConvert.DeserializeObject<MessageRollbackJson>(json);
                var command = new MessageRollback(commandParsed.Speech, commandParsed.Character, commandParsed.Attributes);
                command.SetPosition(commandParsed);
                return command;
            });

			config.DeserializeParsers.Add("python", (json, core) =>
			{
				PythonJson commandParsed = JsonConvert.DeserializeObject<PythonJson>(json);
				var command = new Python() { Commands = commandParsed.Commands.ToList() };
				command.SetPosition(commandParsed);
				return command;
			});


            return config;
		}

	}
}
