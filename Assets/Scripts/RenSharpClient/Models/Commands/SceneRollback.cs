using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Commands.Results;
using RenSharpClient.Controllers;
using System.Collections.Generic;

namespace RenSharpClient.Models.Commands
{
	public class SceneRollback : Command
	{
		public ImageController Controller { get; set; }
		public IEnumerable<ShowResult> ImagesBeforeScene { get; set; }

		public SceneRollback(ImageController controller, IEnumerable<ShowResult> imagesBeforeScene)
		{
			Controller = controller;

			if (imagesBeforeScene == null)
				imagesBeforeScene = new List<ShowResult>();
			ImagesBeforeScene = imagesBeforeScene;
		}

		public override void Execute(RenSharpCore core)
		{
			Controller.HideAll();
			foreach(ShowResult show in ImagesBeforeScene)
			{
				show.attributes.Remove("with");
				Controller.Show(show, core.Configuration, core);
			}
		}
	}
}
