using RenSharp.Core;
using RenSharp.Interfaces;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
	public class While : Command, IPushable
	{
		public string Expression { get; set; }
		public While(string expression)
		{
			Expression = expression;
		}

		public override void Execute(RenSharpCore core)
		{
			
		}

		public bool Push(RenSharpContext ctx)
		{
			bool result = ctx.Evaluate<bool>(Expression);
			if (result)
			{
				ctx.LevelStack.Push(Line);
				return true;
			}
			return false;
		}
		
	}
}
