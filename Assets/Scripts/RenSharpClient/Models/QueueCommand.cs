using RenSharp.Core;
using RenSharpClient.Controllers;
using RenSharpClient.Models.Commands.Results;
using System.Collections.Generic;

namespace RenSharp.Models
{
    internal class QueueCommand : Command
    {
        public IEnumerable<string> ClipNames { get; set; }
        public string Channel { get; set; }
        public Attributes Attributes { get; set; }
        public SoundController Controller { get; set; }

        public QueueCommand(IEnumerable<string> clipNames, string channel, Attributes attributes, SoundController controller)
        {
            ClipNames = clipNames;
            Channel = channel;
            Controller = controller;
            Attributes = attributes;
        }

        public override void Execute(RenSharpCore core)
        {
            PlayResult result = new PlayResult()
            {
                Attributes = Attributes,
                Channel = Channel,
                ClipNames = ClipNames
            };
            Controller.Enqueue(result);
        }
    }
}
