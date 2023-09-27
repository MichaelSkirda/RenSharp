using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Call : Command
	{
		public string Expression { get; set; }

		public Call(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentNullException("Call must have at least 1 argument (label name).");
			Expression = expression;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			ctx.PushState();
			string labelName = ctx.InterpolateString(Expression);
			labelName = ctx.ExecuteExpression<string>(labelName);
			renSharpCore.Goto(labelName);
		}
	}
}
