using System.Text.RegularExpressions;

namespace RenSharp.Core.Parse
{
	public static class RegexMethods
	{
		public static bool IsValidCharacterName(string name)
		{
			Regex regex = new Regex(@"^[a-zA-Z_]+[a-z-A-Z0-9 _]*$");
			return regex.IsMatch(name);
		}
	}
}
