using System.Collections.Generic;
using RenSharp.Core;
using RenSharp.Interfaces;

namespace RenSharp.Models.Commands
{
    public class Label : Command, IPushable
	{
		public string Name { get; set; }

		public Label(string name)
		{
			Name = name;
		}

		public override void Execute(RenSharpCore core)
		{

		}
		public bool Push(RenSharpContext ctx)
		{
			ctx.LevelStack.Push(0);
			return true;
		}
	}
}
