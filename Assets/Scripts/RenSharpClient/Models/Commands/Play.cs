using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;

namespace RenSharpClient.Models.Commands
{
	internal class Play : Command
	{
		public string Name { get; set; }
		public bool IsMusic { get; set; }
		public SoundController Controller { get; set; }

		public Play(string name, bool isMusic, SoundController controller)
		{
			Name = name;
			IsMusic = isMusic;
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			if (IsMusic)
				Controller.PlayMusic(Name);
			else
				Controller.PlaySound(Name);
		}
	}
}
