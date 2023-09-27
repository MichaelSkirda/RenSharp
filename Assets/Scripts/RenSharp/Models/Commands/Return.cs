using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Return : Command
	{
		public string Expression { get; set; }
		public bool IsSoft { get; set; }

		public Return(string expression, bool isSoft)
		{
			Expression = expression;
			IsSoft = isSoft;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			string exp = ctx.InterpolateString(Expression);
			ctx.Variables["_return"] = ctx.ExecuteExpression<object>(exp);

			// Soft return won't pop last element
			if (IsSoft && ctx.Stack.Count <= 1)
				return;

			ctx.PopState();
		}
	}
}
