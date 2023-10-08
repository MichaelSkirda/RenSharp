using RenSharp.Models;

namespace RSTests.CommandTests
{
	public class MathTests : TestBase
	{

		[Theory]
		[InlineData("./CommandTests/MathTests/5_plus_3.csren")]
		[InlineData("./CommandTests/MathTests/brackets_test.csren")]
		public void Tests(string path)
		{
			ExecuteUntilExit(path);
			List<MessageResult> messages = Messages;
			int expected = RS.GetVariable<int>("expected");
			int actual = RS.GetVariable<int>("actual");

			Assert.Empty(messages);
			Assert.Equal(expected, actual);
		}

	}
}
