using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Repeat : Command
	{
		public string Expression { get; set; }
		public int? Times { get; private set; } = null;
		private int Repeated { get; set; } = 0;

		public Repeat(string expression)
		{
			Expression = expression;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			if(Times == null)
				Times = context.ExecuteExpression<int>(Expression);

			
			Repeated++;
			if (Repeated >= Times.Value)
			{
				Times = null;
				context.LevelStack.Push(0);
				Repeated = 0;
			}
			else
			{
				context.LevelStack.Push(Line);
			}
		}
	}
}
