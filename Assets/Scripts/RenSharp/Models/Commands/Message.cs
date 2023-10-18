﻿using RenSharp.Core;
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
			Attributes attributes = core.GetCharacterAttributes(Character);
			attributes.AddAttributes(Attributes);

			MessageResult result = new MessageResult()
			{
				Speech = core.Context.InterpolateString(Speech),
				Character = Character,
				Attributes = attributes
			};

			IWriter writer = core.Configuration.Writer;
			if (writer != null)
				writer.Write(result);
		}

		public override Command Rollback()
		{
			var command = new Message(Speech, Character, RawAttributes);

			command.Line = Line;
			command.SourceLine = SourceLine;
			command.Level = Level;

			return command;
		}
	}
}
