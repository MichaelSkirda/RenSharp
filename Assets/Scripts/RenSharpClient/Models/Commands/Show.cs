using RenSharp;
using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Commands.Results;
using RenSharpClient.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace RenSharpClient.Models.Commands
{
	public class Show : Command, IRollbackable
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
			Controller.Show(character, config, core);
		}

		public virtual Command Rollback(RenSharpCore core)
		{
            IEnumerable<ShowResult> imagesBeforeScene = Controller.GetActiveSprites()
                .Select(x => new ShowResult(x.Name, x.Details, x.Attributes))
				.ToList();
            var sceneRollback = new SceneRollback(Controller, imagesBeforeScene);
            sceneRollback.SetPosition(this);
            return sceneRollback;
            /* Attributes attributes = Attributes.Empty();
			var hide = new Hide(Name, attributes, Controller);
			hide.SetPosition(this);
			return hide; */
        }
	}
}
