namespace RSTests.RSAssert.Exceptions
{
	internal class AssertException : Exception
	{
		internal object Expected { get; set; }
		internal object Actual { get; set; }

		public AssertException(string message, object expected, object actual)
			: base($"{message}\nExpected: '{expected}'. Actual: '{actual}'.")
		{
			Expected = expected;
			Actual = actual;
		}

	}
}
