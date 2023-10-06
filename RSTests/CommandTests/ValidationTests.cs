using RenSharp.Core.Exceptions;

namespace RSTests.CommandTests
{
	public class ValidationTests : TestBase
	{ 
        public ValidationTests() : base() { }

		[Theory]
		[InlineData("./CommandTests/ValidationTests/nostart.csren")]
		[InlineData("./CommandTests/ValidationTests/empty.csren")]
		public void NoStartTests(string path)
		{
			var ex = Assert.Throws<LabelNotExists>(() => ExecuteUntilExit(path));
		}
	}
}
