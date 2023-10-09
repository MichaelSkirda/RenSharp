using RenSharp.Models;

namespace Assets.Scripts.RenSharpClient.Commands.Results
{
	internal class ShowResult
	{
		internal string Name { get; set; }
		internal string Details { get; set; }
		internal Attributes attributes { get; set; }

		public ShowResult(string name, Attributes attributes)
		{
			Name = name;
			this.attributes = attributes;
		}
	}
}
