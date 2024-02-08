using RenSharp.Models;

namespace RSTests.CommandTests
{
	public class RepeatTests : TestBase
	{
		public RepeatTests() : base() { }

		[Theory]
		[InlineData("./CommandTests/RepeatTests/repeat_5.csren")]
		[InlineData("./CommandTests/RepeatTests/repeat_5_byvar.csren")]
		[InlineData("./CommandTests/RepeatTests/repeat_5_byvar_init.csren")]
		public void RepeatFiveTimes(string path)
		{
			ExecuteUntilExit(path);
			List<MessageResult> messages = Messages;

			Assert.Equal(5, messages.Count());
			Assert.True(messages.All(x => x.Character == "_rs_nobody_character"));
			Assert.True(messages.All(x => x.Speech == "Hello!"));
		}
	}
}
