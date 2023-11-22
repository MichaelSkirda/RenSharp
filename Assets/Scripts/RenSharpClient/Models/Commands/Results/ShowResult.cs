using RenSharp.Models;

namespace RenSharpClient.Commands.Results
{
	public class ShowResult
	{
		public string Name { get; set; }
		public string Details { get; set; }
		public Attributes attributes { get; set; }

		public ShowResult(string name, string details, Attributes attributes)
		{
			Name = name;
			Details = details;
			this.attributes = attributes;
		}
	}
}
