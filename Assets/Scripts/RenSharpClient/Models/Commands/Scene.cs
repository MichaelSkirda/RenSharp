using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Commands.Results;
using RenSharpClient.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace RenSharpClient.Models.Commands
{
	public class Scene : Show
	{
		public Scene(string name, string details, Attributes attributes, ImageController controller)
			: base(name, details, attributes, controller) { }

		public override void Execute(RenSharpCore core)
		{
			Controller.HideAll();
			base.Execute(core);
		}

		public override Command Rollback(RenSharpCore core)
		{
			IEnumerable<ShowResult> imagesBeforeScene = Controller.GetActiveSprites()
				.Select(x => new ShowResult(x.Name, x.Details, x.Attributes));
			var sceneRollback = new SceneRollback(Controller, imagesBeforeScene);
			sceneRollback.SetPosition(this);
			return sceneRollback;
		}
	}
}
