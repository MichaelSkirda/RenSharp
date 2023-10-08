namespace RSTests.CommandTests
{
	public class WhileTests : TestBase
	{

		[Theory]
		[InlineData("./CommandTests/WhileTests/while_10times.csren")]
		public void while_10times(string path)
		{
			ExecuteUntilExit(path);
		}
	}
}
