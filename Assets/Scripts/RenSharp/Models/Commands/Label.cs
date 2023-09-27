using System;
using System.Collections.Generic;
using System.Text;
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

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			Push(ctx.LevelStack, ctx);
		}
		public void Push(Stack<int> stack, RenSharpContext ctx) => stack.Push(0);
	}
}
