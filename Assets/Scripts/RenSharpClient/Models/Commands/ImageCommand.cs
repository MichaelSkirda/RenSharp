using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;
using RenSharpClient.Models.Commands.Results;

namespace RenSharpClient.Models.Commands
{
	public class ImageCommand : Command
	{
		public string Name { get; set; }
		public string Details { get; set; }
		public Attributes Attributes { get; set; }
		public ImageController Controller { get; set; }

		public ImageCommand(string name, string details, Attributes attributes, ImageController controller)
		{
			Name = name;
			Details = details;
			Attributes = attributes;
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			string width = Attributes.GetValueOrNull("width");
			string height = Attributes.GetValueOrNull("height");
			float? zoom = Attributes.GetFloatOrNull("zoom");
			if (zoom == null)
				zoom = 1f;

			var image = new ImageResult()
			{
				Width = width,
				Height = height,
				Zoom = zoom.Value,
				Name = Name,
				Details = Details
			};

			Controller.SetSize(image);
		}
	}
}
