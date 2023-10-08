using RenSharp.Core;
using RenSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class While : Command, IPushable
	{
		private string Expression { get; set; }
		public While(string expression)
		{
			Expression = expression;
		}

		internal override void Execute(RenSharpCore core)
		{
			var ctx = core.Context;
			bool result = ctx.Evaluate<bool>(Expression);
			if (result)
				Push(ctx.LevelStack, ctx);
		}

		public void Push(Stack<int> stack, RenSharpContext ctx) => stack.Push(Line);
		
	}
}
