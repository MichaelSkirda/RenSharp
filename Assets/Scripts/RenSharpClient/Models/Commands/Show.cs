using Assets.Scripts.RenSharpClient.Commands.Results;
using RenSharp;
using RenSharp.Core;
using RenSharp.Models;

namespace Assets.Scripts.RenSharpClient.Models.Commands
{
	public class Show : Command
	{
		public string Name { get; set; }
		public string Details { get; set; }
		public Attributes Attributes { get; set; }
		public ImageController Controller { get; set; }

		public Show(string name, string details, Attributes attributes, ImageController controller)
		{
			Name = name;
			Details = details;
			Attributes = attributes;
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			Configuration config = core.Configuration;

			Attributes.AddDefaultAttributes(config);
			var character = new ShowResult(Name, Details, Attributes);
			Controller.Show(character, config);
		}
	}
}
