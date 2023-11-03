using RenSharp;
using RenSharp.Models;
using RSTests.RSAssert;
using System.Reflection;

namespace RSTests
{
	public class TestBase
	{
		protected static RenSharpCore RS { get; set; }
		internal TestWriter Writer { get; set; }
		internal List<MessageResult> Messages => Writer.WritedMessage;

		static TestBase()
		{
			RS = new RenSharpCore();
		}
		public TestBase()
		{
			Writer = new TestWriter();
		}

		protected void ExecuteUntilExit(string path)
		{
			RS.LoadProgram(path, false);
			
			Writer = new TestWriter();
			RenSharpAssert assert = CreateAssert();

			RS.Configuration.Writer = Writer;
			RS.SetVariable("Assert", assert);

			while (true)
			{
				Command? command = RS.ReadNext();

				if (command is Exit)
					break;
			}
		}

		private RenSharpAssert CreateAssert()
		{
			var messagesAssert = new MessagesAssert(Messages);
			var assert = new RenSharpAssert(messagesAssert);
			return assert;
		}
	}
}
