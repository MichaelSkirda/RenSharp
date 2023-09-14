using RenSharp.Core;
using RenSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
    public class Message : Command
	{
		private string RawLine { get; set; }
		public string Speech { get; set; }
		public string Character { get; set; }
		public Attributes Attributes { get; set; }

		public Message(string speech, string character, IEnumerable<string> attributes = null)
		{
			Speech = speech;
			RawLine = speech;
			Character = character;
			Attributes = new Attributes(attributes);
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			Speech = context.ReplaceVars(RawLine);

			Attributes attributes = renSharpCore.GetCharacterAttributes(Character);
			Attributes.AddAttributes(attributes);

			IWriter writer = renSharpCore.Writer;
			if (writer != null)
				writer.Write(this);
		}
	}
}
