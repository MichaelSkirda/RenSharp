using System;
using RenSharp.Core;

namespace RenSharp.Models.Commands
{
    public class Jump : Command
	{
		public string Expression { get; set; }
		public bool Evaluate { get; set; }

		public Jump(string expression, bool evaluate)
		{
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentNullException("Goto label can not be null or empty");
			Expression = expression;
			Evaluate = evaluate;
		}

		public override void Execute(RenSharpCore core)
		{
			string labelName = Expression;
			if (Evaluate)
			{
				// TODO I don't know why there is InterpolateString. Find out why.
				//labelName = core.Context.InterpolateString(Expression);
				labelName = core.Context.Evaluate<string>(labelName);
			}
			core.Goto(labelName);
		}
	}
}
