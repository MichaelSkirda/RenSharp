using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Call : Command
	{
		public string Expression { get; set; }
		public bool Evaluate { get; set; }

		public Call(string expression, bool evaluate)
		{
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentNullException("Call must have at least 1 argument (label name).");
			Expression = expression;
			Evaluate = evaluate;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			ctx.PushState();
			string labelName = Expression;

			if (Evaluate)
				labelName = ctx.ExecuteExpression<string>(Expression);

			renSharpCore.Goto(labelName);
		}
	}
}
