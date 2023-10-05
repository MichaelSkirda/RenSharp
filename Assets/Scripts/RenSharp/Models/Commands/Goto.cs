using System;
using System.Collections.Generic;
using System.Text;
using RenSharp.Core;

namespace RenSharp.Models.Commands
{
    public class Goto : Command
	{
		public string Expression { get; set; }
		public bool Evaluate { get; set; }

		public Goto(string expression, bool evaluate)
		{
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentNullException("Goto label can not be null or empty");
			Expression = expression;
			Evaluate = evaluate;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			string labelName = Expression;
			if (Evaluate)
			{
				labelName = ctx.InterpolateString(Expression);
				labelName = ctx.ExecuteExpression<string>(labelName);
			}
			renSharpCore.Goto(labelName);
		}
	}
}
