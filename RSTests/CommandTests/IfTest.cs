using RenSharp.Models;

namespace RSTests.CommandTests
{
    public class IfTest : TestBase
    {
        public IfTest() : base() { }

        [Theory]
        [InlineData("./CommandTests/IfTests/5_biggerThan_2_initPy.csren")]
        [InlineData("./CommandTests/IfTests/5_biggerThan_2_initPyBlock.csren")]
        [InlineData("./CommandTests/IfTests/5_biggerThan_2_initPyol.csren")]
        [InlineData("./CommandTests/IfTests/5_biggerThan_2_pyol.csren")]
        [InlineData("./CommandTests/IfTests/5_biggerThan_2_rsset.csren")]
        [InlineData("./CommandTests/IfTests/5_biggerThan_2_rssetinit.csren")]
        public void Five_BiggerThan_Two(string path)
        {
            ExecuteUntilExit(path);
            MessageResult message = Messages.First();

            Assert.Single(Messages);
            Assert.Equal("_rs_nobody_character", message.Character);
            Assert.Equal("x bigger than 5", message.Speech);
        }
    }
}