using RenSharp.Models.Parse;
using System.Text.RegularExpressions;

namespace RenSharp.Core.Parse
{
	public static class RegexMethods
	{
		// All values between quotes. Ignores escaped \" quotes.
		private static Regex ValueInQuotes = new Regex(@"""(?:[^""\\]|\\.)*""");

		public static bool IsValidCharacterName(string name)
		{
			Regex regex = new Regex(@"^[a-zA-Z_]+[a-z-A-Z0-9 _]*$");
			return regex.IsMatch(name);
		}

		public static StringFirstQuotes BetweenQuotesFirst(string[] words)
			=> BetweenQuotesFirst(string.Join(' ', words));
		public static StringFirstQuotes BetweenQuotesFirst(string text)
		{
			Match match = ValueInQuotes.Match(text);

			string before = null;
			string between = null;
			string after = null;

			if (match.Captures.Count > 0)
			{
				before = text.Substring(0, match.Index).Trim(); // 'say Eliz'
				if (match.Value.Length >= 2)
				{
					// Substring to not include quotes (") symbols. We ignore first and last quote (")
					between = match.Value.Substring(1, match.Value.Length - 2).Trim().Replace("\\\"", "\"");
				}
				else
				{
					between = string.Empty;
				}
				after = text.Substring(match.Index + match.Length).Trim();   // 'no-clear delay=50'
			}

			return new StringFirstQuotes()
			{
				Before = before,
				Between = between,
				After = after,
				RegexMatch = match
			};
		}

		public static int? IndexOfComment(string line)
		{
			var regex = new Regex(@"\#(?=([^""]*""[^""]*"")*[^""]*$)");
			Match match = regex.Match(line);
			if (match.Success)
				return match.Index;
			return null;
		}
	}
}
