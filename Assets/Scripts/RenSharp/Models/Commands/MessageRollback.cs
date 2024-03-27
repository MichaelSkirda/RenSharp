using RenSharp.Core;

namespace RenSharp.Models.Commands
{
	internal class MessageRollback : Message
	{
		public MessageRollback(string speech, string character, Attributes attributes = null)
			: base(speech, character, attributes) { }


        // TODO: bugfix!
        // MessageRollback Pop() stack.
        // But base.Execute(core) Push() stack.
        // Next rollbacked MessageRollback will Pop
        // previous Pop() by MessageRollback.
        //
        // Use Guid?

        public override void Execute(RenSharpCore core)
		{
			core.Context.MessageHistory.Pop();
			WriteMessage(core);
		}
	}
}
