using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;
using RenSharpClient.Models.Commands.Results;

namespace RenSharpClient.Models.Commands
{
	internal class Play : Command
	{
		public string Name { get; set; }
		public string Channel { get; set; }
		public Attributes Attributes { get; set; }
		public SoundController Controller { get; set; }

		public Play(string name, string channel, Attributes attributes, SoundController controller)
		{
			Name = name;
			Channel = channel;
			Controller = controller;
			Attributes = attributes;
		}

		public override void Execute(RenSharpCore core)
		{
			var playResult = new PlayResult()
			{
				Name = Name,
				Channel = Channel,
				Attributes = Attributes
			};
			Controller.Play(playResult);
		}
	}
}
