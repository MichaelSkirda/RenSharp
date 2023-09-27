using System;
using System.Collections.Generic;
using System.Text;
using RenSharp.Core;

namespace RenSharp.Models.Commands
{
    public class Goto : Command
	{
		public string Expression { get; set; }

		public Goto(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentNullException("Call must have at least 1 argument (label name).");
			Expression = expression;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			string labelName = ctx.InterpolateString(Expression);
			labelName = ctx.ExecuteExpression<string>(labelName);
			renSharpCore.Goto(labelName);
		}
	}
}
