using Assets.Scripts.RenSharpClient.Models.Commands.Results;
using RenSharp.Core;
using RenSharp.Models;

namespace Assets.Scripts.RenSharpClient.Models.Commands
{
	public class Image : Command
	{
		public string Name { get; set; }
		public string Details { get; set; }
		public Attributes Attributes { get; set; }
		public ImageController Controller { get; set; }

		public Image(string name, string details, Attributes attributes, ImageController controller)
		{
			Name = name;
			Details = details;
			Attributes = attributes;
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			int? width = Attributes.GetIntOrNull("width");
			int? height = Attributes.GetIntOrNull("height");
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
