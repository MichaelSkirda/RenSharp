using RenSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace RenSharp.Core
{
	public class MessageHistory
	{
		private Stack<MessageResult> Messages {  get; set; }

		internal MessageHistory()
		{
			Messages = new Stack<MessageResult>();
		}

		public void Push(MessageResult message)
			=> Messages.Push(message);
		public void Pop()
			=> Messages.Pop();
		public void Clear()
			=> Messages.Clear();

		public IEnumerable<MessageResult> All()
			=> Messages.ToList();
	}
}
