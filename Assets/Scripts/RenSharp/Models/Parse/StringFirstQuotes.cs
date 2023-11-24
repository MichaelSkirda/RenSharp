using System.Text.RegularExpressions;

namespace RenSharp.Models.Parse
{
	public class StringFirstQuotes
	{
		public string Before { get; set; }
		public string Between { get; set; }
		public string After { get; set; }
		public Match RegexMatch { get; set; }
	}
}
