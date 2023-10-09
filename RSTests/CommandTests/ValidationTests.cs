using RenSharp.Core.Exceptions;

namespace RSTests.CommandTests
{
	public class ValidationTests : TestBase
	{ 
        public ValidationTests() : base() { }

		[Theory]
		[InlineData("./CommandTests/ValidationTests/nostart.csren")]
		[InlineData("./CommandTests/ValidationTests/empty.csren")]
		[InlineData("./CommandTests/ValidationTests/explicit_exit.csren")]
		public void NoStartTests(string path)
		{
			var ex = Assert.Throws<LabelNotExists>(() => ExecuteUntilExit(path));
		}
	}
}
