using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;

namespace RenSharpClient.Models.Commands
{
	internal class MenuRollback : Command
	{
		public MenuController Controller { get; set; }

		public MenuRollback(MenuController controller)
		{
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			Controller.Clear();
			core.Resume();
		}
	}
}
