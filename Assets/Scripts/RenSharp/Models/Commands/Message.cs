using RenSharp.Core;
using RenSharp.Interfaces;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
    public class Message : Command
	{
		private string Speech { get; set; }
		public string Character { get; set; }
		public Attributes Attributes { get; set; }
		public IEnumerable<string> RawAttributes { get; set; }

		public Message(string speech, string character, IEnumerable<string> attributes = null)
		{
			Speech = speech;
			Character = character;
			Attributes = new Attributes(attributes);
			RawAttributes = attributes;
		}

		public override void Execute(RenSharpCore core)
		{
			Configuration config = core.Configuration;
			Attributes characterAttributes = core.GetCharacterAttributes(Character);
			characterAttributes.AddAttributes(Attributes);
			characterAttributes.AddDefaultAttributes(config);

			MessageResult result = new MessageResult()
			{
				Speech = core.Context.InterpolateString(Speech),
				Character = Character,
				Attributes = characterAttributes
			};

			core.Context.MessageHistory.Push(result);

			IWriter writer = config.Writer;
			if (writer != null)
				writer.Write(result);
		}

		public override Command Rollback(RenSharpCore core)
		{
			var command = new MessageRollback(Speech, Character, RawAttributes);

			command.Line = Line;
			command.SourceLine = SourceLine;
			command.Level = Level;

			return command;
		}
	}
}
