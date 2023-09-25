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
		internal static Label ParseLabel(string[] args) => new Label(args[1]);
		internal static Pass ParsePass(string[] args) => (args.Length == 1 && args[0] == "pass")
			? new Pass() : throw new ArgumentException($"Can not parse pass at line '{string.Join(" ", args)}'");
		internal static Goto ParseGoto(string[] args) => new Goto(args[1]);
		internal static Load ParseLoad(string[] args) => new Load(String.Join(" ", args.Skip(1)).GetStringBetween("\""));
		internal static Callback ParseCallback(string[] args) => new Callback(String.Join(" ", args.Skip(1)));
		internal static While ParseWhile(string[] args) => new While(String.Join(" ", args.Skip(1)));
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
			int length = line.Length;

			string command = line[0].Trim(); // 'say Eliz'
			string text = string.Join("\"", line.Skip(1).SkipLast(1));
			string attrs = line.Last().Trim();   // 'no-clear delay=50'

			string[] attributes = attrs
				.Split(' ')
				.Where(x => string.IsNullOrWhiteSpace(x) == false)
				.ToArray();   // 'no-clear delay=50' -> ['no-clear', 'delay=50']
			string character = command.Split(' ')[1]; // 'say Eliz' -> 'Eliz'

			Message message = new Message(text, character, attributes);

			// They won't rewrite current attributes
			List<string> defaultAttributes = new List<string>()
			{
				config.GetDefaultKeyValueString("delay"),
				config.GetDefaultKeyValueString("name")
			};
			message.Attributes.AddAttributes(defaultAttributes, rewrite: false);

			return message;
		}

		internal static Repeat ParseRepeat(string[] words)
		{
			string expression = String.Join(" ", words.Skip(1));

			return new Repeat(expression);
		}
	}
}
