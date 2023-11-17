using RenSharp.Core;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
	internal class MessageRollback : Message
	{
		public MessageRollback(string speech, string character, IEnumerable<string> attributes = null)
			: base(speech, character, attributes) { }

		public override void Execute(RenSharpCore core)
		{
			core.Context.MessageHistory.Pop();
		}
	}
}
