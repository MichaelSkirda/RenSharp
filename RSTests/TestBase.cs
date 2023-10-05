using RenSharp.Models;

namespace RSTests
{
	public class TestBase
	{
		protected static RenSharpCore? RS { get; set; }
		internal TestWriter Writer { get; set; }
		internal List<Message> Messages => Writer.WritedMessage;

		public TestBase()
		{
			Writer = new TestWriter();
		}

		protected void ExecuteUntilExit(string path)
		{
			Writer = new TestWriter();
			RS = new RenSharpCore(path);
			RS.Writer = Writer;

			if (RS == null)
				throw new ArgumentException("RenSharpCore is null!");

			Command? command = null;
			while((command is Exit) == false)
			{
				command = RS.ReadNext();
			}
		}


	}
}
