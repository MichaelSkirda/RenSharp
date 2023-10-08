using RenSharp.Interfaces;
using RenSharp.Models;

namespace RSTests
{
	internal class TestWriter : IWriter
	{
		internal List<MessageResult> WritedMessage { get; set; } = new List<MessageResult>();

		public void Write(MessageResult message)
		{
			WritedMessage.Add(message);
		}
	}
}
