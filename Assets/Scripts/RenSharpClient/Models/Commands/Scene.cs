using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;

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
	}
}
