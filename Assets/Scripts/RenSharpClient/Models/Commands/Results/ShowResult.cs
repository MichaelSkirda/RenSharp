using RenSharp.Models;

namespace RenSharpClient.Commands.Results
{
	internal class ShowResult
	{
		internal string Name { get; set; }
		internal string Details { get; set; }
		internal Attributes attributes { get; set; }

		public ShowResult(string name, string details, Attributes attributes)
		{
			Name = name;
			Details = details;
			this.attributes = attributes;
		}
	}
}
