using RenSharp.Core;
using System;

namespace RenSharp.Models.Commands
{
	public class Call : Command
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

		public override void Execute(RenSharpCore core)
		{
			core.Context.PushState();
			string labelName = Expression;

			if (Evaluate)
				labelName = core.Context.Evaluate<string>(Expression);

			core.Goto(labelName);
		}
	}
}
