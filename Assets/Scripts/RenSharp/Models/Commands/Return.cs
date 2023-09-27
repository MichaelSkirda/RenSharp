using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Return : Command
	{
		public string Expression { get; set; }

		public Return(string expression)
		{
			Expression = expression;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			ctx.Variables["_return"] = ctx.ExecuteExpression<object>(Expression);
			ctx.PopState();
		}
	}
}
