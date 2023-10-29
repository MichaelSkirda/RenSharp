using Assets.Scripts.RenSharpClient.Commands.Results;
using RenSharp.Core;
using RenSharp.Models;
using System.Collections.Generic;

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
			Attributes.AddDefaultAttributes(core.Configuration);
			ShowResult character = new ShowResult(Name, Details, Attributes);
			Controller.Show(character);
		}
	}
}
