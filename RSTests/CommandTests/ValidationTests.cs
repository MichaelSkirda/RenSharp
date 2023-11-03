using RenSharp.Core.Exceptions;
using System.Data;

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

		[Theory]
		[InlineData("./CommandTests/ValidationTests/repeating_starts.csren")]
		[InlineData("./CommandTests/ValidationTests/repeating_labels.csren")]
		public void RepeatingLabels(string path)
		{
			var ex = Assert.Throws<SyntaxErrorException>(() => ExecuteUntilExit(path));
		}

		[Theory]
		[InlineData("./CommandTests/ValidationTests/init_1000.csren")]
		[InlineData("./CommandTests/ValidationTests/init_minus_1000.csren")]
		public void InitWrongPriority(string path)
		{
			var ex = Assert.Throws<SyntaxErrorException>(() => ExecuteUntilExit(path));
			Assert.Equal("Приоритет блока 'init' не может быть более 999 или менее -999.", ex.InnerException?.Message);
		}
	}
}
