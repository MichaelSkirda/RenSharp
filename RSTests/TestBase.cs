using RenSharp.Models;
using RenSharp.Models.Commands;

namespace RSTests
{
	public class TestBase
	{
		protected RenSharpCore? RS { get; set; }
		internal TestWriter Writer { get; set; }
		internal List<Message> Messages => Writer.WritedMessage;

		public TestBase()
		{
		}

		protected void ExecuteUntilExit()
		{
			Writer = new TestWriter();
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
