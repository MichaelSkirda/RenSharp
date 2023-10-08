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
			string? assembly = Assembly.GetExecutingAssembly().FullName;
			if (assembly == null)
				throw new NullReferenceException("Can not find currect assembly for tests.");
			Writer = new TestWriter();
			RenSharpAssert assert = CreateAssert();

			RS.Writer = Writer;
			RS.SetVariable("Assert", assert);

			if (RS == null)
				throw new ArgumentException("RenSharpCore is null!");

			Command? command = null;
			while((command is Exit) == false)
			{
				try
				{
					command = RS.ReadNext();
				}
				catch(Exception ex)
				{
					throw;
				}
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
