using RenSharp.Models;
using RenSharp.Models.Commands;

namespace RSTests
{
	public class TestBase
	{
		protected RenSharpCore? RS { get; set; }

		public TestBase()
		{

		}

		protected void ExecuteUntilExit()
		{
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
