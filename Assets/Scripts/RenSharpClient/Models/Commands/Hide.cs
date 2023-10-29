using RenSharp.Core;
using RenSharp.Models;

namespace Assets.Scripts.RenSharpClient.Models.Commands
{
	internal class Hide : Command
	{
		public string Name { get; set; }
		public ImageController Controller { get; set; }

		public Hide(string name, ImageController controller)
		{
			Name = name;
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			Controller.Hide(Name);
		}
	}
}
