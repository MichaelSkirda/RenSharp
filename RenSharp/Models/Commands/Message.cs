using RenSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	public class Message : Command
	{
		public string Speech { get; set; }
		public string Character { get; set; }
		public List<string> Effects { get; set; }

		public Message(string speech, string character, List<string> effects = null)
		{
			Speech = speech;
			Character = character;
			Effects = effects;
		}

		internal override void Execute(RenSharpCore renSharpCore)
		{
			var CharEffects = renSharpCore.GetCharacterAttributes(Character);
			Effects.AddRange(CharEffects);

			IWriter writer = renSharpCore.Writer;
			if (writer != null)
				writer.Write(Speech);
		}
	}
}
