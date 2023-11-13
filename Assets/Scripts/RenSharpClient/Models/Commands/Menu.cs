using RenSharp.Core;
using RenSharp.Models;
using RenSharp.RenSharpClient.Models;
using System.Collections.Generic;

namespace Assets.Scripts.RenSharpClient.Models.Commands
{
	public class Menu : Command
	{
		public ICollection<MenuButton> Buttons { get; set; } = new List<MenuButton>();

		public Menu()
		{
		
		}

		public override void Execute(RenSharpCore core)
		{
			throw new System.NotImplementedException();
		}
	}
}
