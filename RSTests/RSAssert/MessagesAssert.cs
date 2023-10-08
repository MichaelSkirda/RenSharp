using RenSharp.Models;
using RSTests.RSAssert.Exceptions;

namespace RSTests.RSAssert
{
	public class MessagesAssert
	{
		private List<MessageResult> Messages { get; set; }

		public MessagesAssert(List<MessageResult> messages)
		{
			Messages = messages;
		}

		public void Count(int expected)
		{
			int actual = Messages.Count;
			if (actual != expected)
				throw new AssertException("Wrong messages count.", expected, actual);
		}

		public void Exact(IEnumerable<string> msg)
		{
			List<string> actual = Messages.Select(x => x.Speech).ToList();
			List<string> expected = msg.ToList();

			int expectedCount = expected.Count();
			int actualCount = actual.Count();

			if (expectedCount != actualCount)
				throw new AssertException("Wrong messages count.", expectedCount, actualCount);

			for(int i = 0; i < expectedCount; i++)
			{
				if (expected[i] != actual[i])
					throw new AssertException($"Message with index {i} wrong.", expected[i], actual[i]);
			}
		}
	}
}
