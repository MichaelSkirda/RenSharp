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
		internal static Goto ParseGoto(string[] args) => new Goto(args[1]);
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

			string[] attributes = attrs.Split(' ');   // 'no-clear delay=50' -> ['no-clear', 'delay=50']
			string character = command.Split(' ')[1]; // 'say Eliz' -> 'Eliz'

			Message message = new Message(text, character, attributes);

			// They won't override current attributes
			List<string> defaultAttributes = new List<string>()
			{
				config.GetDefault("delay"),
			};
			message.Attributes.AddAttributes(defaultAttributes);

			return message;
		}
		
	}
}
