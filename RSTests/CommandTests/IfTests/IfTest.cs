
namespace RSTests.CommandTests.IfTests
{
	[TestClass]
	public class IfTest : TestBase
    {
        public IfTest() : base() { }

		[TestMethod]
		[DataRow("./CommandTests/IfTests/py/5_biggerThan_2.py")]
        public void Five_BiggerThan_Two(string path)
        {
            RS = new RenSharpCore(path);
        }
    }
}