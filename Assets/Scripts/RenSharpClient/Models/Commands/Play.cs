using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;
using RenSharpClient.Models.Commands.Results;
using System.Collections.Generic;

namespace RenSharpClient.Models.Commands
{
	internal class Play : Command, IRollbackable
	{
		public IEnumerable<string> ClipNames { get; set; }
		public string Channel { get; set; }
		public Attributes Attributes { get; set; }
		public SoundController Controller { get; set; }

		public Play(IEnumerable<string> clipNames, string channel, Attributes attributes, SoundController controller)
		{
            ClipNames = clipNames;
			Channel = channel;
			Controller = controller;
			Attributes = attributes;
		}

		public override void Execute(RenSharpCore core)
		{
			var playResult = new PlayResult()
			{
				ClipNames = ClipNames,
				Channel = Channel,
				Attributes = Attributes
			};
			Controller.Play(playResult);
		}

        public Command Rollback(RenSharpCore core)
        {
			return null;
        }
    }
}
