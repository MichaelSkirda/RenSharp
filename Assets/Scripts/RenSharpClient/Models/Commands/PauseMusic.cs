﻿using RenSharp.Core;
using RenSharp.Models;
using RenSharpClient.Controllers;
using RenSharpClient.Models.Commands.Results;

namespace RenSharpClient.Models.Commands
{
    internal class PauseMusic : Command
    {
        public string Channel { get; set; }
        public Attributes Attributes { get; set; }
        public SoundController Controller { get; set; }

        public PauseMusic(string channel, Attributes attributes, SoundController controller)
        {
            Channel = channel;
            Attributes = attributes;
            Controller = controller;
        }

        public override void Execute(RenSharpCore core)
        {
            var stop = new StopResult()
            {
                Channel = Channel,
                Attributes = Attributes
            };
            Controller.Pause(stop);
        }
    }
}
