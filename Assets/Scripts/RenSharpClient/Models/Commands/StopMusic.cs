using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;

namespace Assets.Scripts.RenSharpClient.Models.Models.Commands
{
	internal class StopMusic : Command
	{
		public SoundController Controller { get; set; }

		public StopMusic(SoundController controller)
		{
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			Controller.PauseMusic();
		}
	}
}
