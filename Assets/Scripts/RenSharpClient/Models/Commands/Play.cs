using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;

namespace RenSharpClient.Models.Commands
{
	internal class Play : Command
	{
		public string Name { get; set; }
		public string Channel { get; set; }
		public SoundController Controller { get; set; }

		public Play(string name, string channel, SoundController controller)
		{
			Name = name;
			Channel = channel;
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			if (Channel == "music")
				Controller.PlayMusic(Name);
			else
				Controller.PlaySound(Name);
		}
	}
}
