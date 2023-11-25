using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;
using System.Linq;

namespace RenSharpClient.Models.Commands
{
	internal class Hide : Command
	{
		public string Name { get; set; }
		Attributes Attributes { get; set; }
		public ImageController Controller { get; set; }

		public Hide(string name, Attributes attributes, ImageController controller)
		{
			Name = name;
			Attributes = attributes;
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			Controller.Hide(Name, core, Attributes);
		}

		public override Command Rollback(RenSharpCore core)
		{
			Attributes attributes = Attributes.Empty();
			ActiveSprite sprite = Controller.ActiveSprites
				.Values
				.Where(x => x.Name == Name)
				.FirstOrDefault();

			if (sprite == null)
				return null;

			var show = new Show(Name, sprite.Details, attributes, Controller);
			show.SetPosition(this);
			return show;
		}
	}
}
