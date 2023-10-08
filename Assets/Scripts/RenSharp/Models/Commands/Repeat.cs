using RenSharp.Core;
using RenSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Repeat : Command, IPushable
	{
		public string Expression { get; set; }
		public int? Times { get; private set; } = null;
		private int Repeated { get; set; } = 0;

		public Repeat(string expression)
		{
			Expression = expression;
		}

		internal override void Execute(RenSharpCore core)
		{
			var ctx = core.Context;
			if(Times == null)
				Times = ctx.Evaluate<int>(Expression);

			if (Repeated < Times.Value)
			{
				Push(ctx.LevelStack, ctx);
			}
			else
			{
				Reset();
			}
		}

		private void Reset()
		{
			Times = null;
			Repeated = 0;
		}
		public void Push(Stack<int> stack, RenSharpContext ctx)
		{
			Repeated++;
			stack.Push(Line);
		}
	}
}
