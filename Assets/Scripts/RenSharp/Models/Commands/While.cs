using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class While : Command
	{
		private string Expression { get; set; }
		public While(string expression)
		{
			Expression = expression;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			bool result = context.ExecuteExpression<bool>(Expression);
			if (result)
				context.LevelStack.Push(Line);
		}
	}
}
