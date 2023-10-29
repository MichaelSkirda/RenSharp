using RenSharp.Core;
using RenSharp.Models;
using System.Collections.Generic;

namespace Assets.Scripts.RenSharpClient.Models.Commands
{
	internal class Scene : Command
	{
		public ImageController Controller { get; set; }
		public IEnumerable<ImageShow> Images { get; set; }

		public Scene(ImageController controller, IEnumerable<ImageShow> images)
		{
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			Controller.HideAll();
			foreach(ImageShow image in Images)
			{
				// TODO
				//Controller.ShowCharacter
			}
		}
	}
}
