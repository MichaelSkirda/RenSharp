using RenSharp.Models;
using System.Collections.Generic;

namespace RenSharp.Core
{
	public class MessageHistory
	{
		internal Stack<MessageResult> Messages { get; private set; }

		internal MessageHistory()
		{
			Messages = new Stack<MessageResult>();
		}

		internal MessageHistory(Stack<MessageResult> messages)
		{
			Messages = messages;
		}

		public void Push(MessageResult message)
			=> Messages.Push(message);
		public void Pop()
			=> Messages.Pop();
		public void Clear()
			=> Messages.Clear();
	}
}
