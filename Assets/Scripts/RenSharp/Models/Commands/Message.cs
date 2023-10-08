using RenSharp.Core;
using RenSharp.Interfaces;
using System.Collections.Generic;

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

		internal override void Execute(RenSharpCore core, RenSharpContext ctx)
		{
			Attributes attributes = core.GetCharacterAttributes(Character);
			attributes.AddAttributes(Attributes);

			MessageResult result = new MessageResult()
			{
				RawLine = RawLine,
				Speech = ctx.InterpolateString(RawLine),
				Character = Character,
				Attributes = attributes
			};

			IWriter writer = core.Writer;
			if (writer != null)
				writer.Write(result);
		}
	}
}
