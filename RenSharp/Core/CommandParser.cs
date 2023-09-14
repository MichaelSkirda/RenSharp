using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenSharp.Core
{
	internal static class CommandParser
	{
		internal static Nop ParseNop() => new Nop();
		internal static Label ParseLabel(string[] args) => new Label(args[1]);
		internal static Goto ParseGoto(string[] args) => new Goto(args[1]);
		internal static Load ParseLoad(string[] args) => new Load(String.Join(" ", args.Skip(1)).GetStringBetween("\""));
		internal static Callback ParseCallback(string[] args) => new Callback(String.Join(" ", args.Skip(1)));
		internal static If ParseIf(string[] args)
		{
			bool isRoot;
			if (args[0] == "if")
				isRoot = true;
			else if (args[0] == "else")
				isRoot = false;
			else
				throw new ArgumentException($"Can not parse If {string.Join(" ", args)}");

			string expression;
			if(isRoot)
				expression = String.Join(" ", args.Skip(1));
			else
				expression = String.Join(" ", args.Skip(2));

			expression = expression.Replace("==", "=");

			If command = new If(expression, isRoot);
			return command;
		}
			internal static Set ParseSet(string[] args)
		{
			// Example:
			// set foo=42 ->   foo=42
			// set foo =42 ->  foo =42
			// set foo= 42 ->  foo= 42
			// set foo = 42 -> foo = 42
			// set bar = "Hello world!" -> bar = "Hello world!"
			string command = String.Join(" ", args.Skip(1));

			string[] keyValue = command.Split("=");
			if (keyValue.Length != 2)
				throw new ArgumentException($"Can not parse string {command}.");

			string name = keyValue[0].Trim();
			string expression = keyValue[1].Trim();

			if (name.Contains(" "))
				throw new ArgumentException($"Can not parse command {command}");

			Set set = new Set(name, expression);
			return set;
		}

		internal static Character ParseCharacter(string[] args)
		{
			string name = args[1];
			IEnumerable<string> attributes = args.Skip(2);

			return new Character(name, attributes);
		}
		internal static Message ParseMessage(string[] words, Configuration config)
		{
			// Example:
			// say Eliz "Hello, Alex! How are you?" no-clear delay=50
			// say nobody "Hello, It's test message" delay=20
			//    ^       ^ (splitter)   ^         ^   ^ (index 3)
			// (index 0)              (index 1)  (splitter)
			string[] line = String.Join(" ", words).Split('\"');

			string command = line[0].Trim(); // 'say Eliz'
			string text = line[1];        // 'Hello, Alex! How are you?'
			string attrs = line[2].Trim();   // 'no-clear delay=50'

			string[] attributes = attrs
				.Split(' ')
				.Where(x => string.IsNullOrWhiteSpace(x) == false)
				.ToArray();   // 'no-clear delay=50' -> ['no-clear', 'delay=50']
			string character = command.Split(' ')[1]; // 'say Eliz' -> 'Eliz'

			Message message = new Message(text, character, attributes);

			// They won't override current attributes
			List<string> defaultAttributes = new List<string>()
			{
				config.GetDefault("delay"),
				config.GetDefault("name")
			};
			message.Attributes.AddAttributes(defaultAttributes);

			return message;
		}

		internal static Repeat ParseRepeat(string[] words)
		{
			string expression = String.Join(" ", words.Skip(1));

			return new Repeat(expression);
		}
	}
}
