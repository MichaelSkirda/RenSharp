
namespace RSTests.CommandTests.IfTests
{
	[TestClass]
	public class IfTest : TestBase
    {
        public IfTest() : base() { }

		[TestMethod]
		[DataRow("./CommandTests/IfTests/py/5_biggerThan_2_initPy.csren")]
		[DataRow("./CommandTests/IfTests/py/5_biggerThan_2_initPyBlock.csren")]
		[DataRow("./CommandTests/IfTests/py/5_biggerThan_2_initPyol.csren")]
		[DataRow("./CommandTests/IfTests/py/5_biggerThan_2_pyol.csren")]
		[DataRow("./CommandTests/IfTests/py/5_biggerThan_2_rsset.csren")]
		[DataRow("./CommandTests/IfTests/py/5_biggerThan_2_rssetinit.csren")]
		public void Five_BiggerThan_Two(string path)
        {
            RS = new RenSharpCore(path);
            ExecuteUntilExit();
            Message message = Messages.First();

            Assert.Equals(Messages.Count(), 1);
            Assert.Equals(message.Character, "nobody");
            Assert.Equals(message.Speech, "x bigger than 5");
		}
    }
}