using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	public class Return : Command
	{
		public string Expression { get; set; }
		public bool IsSoft { get; set; }

		public Return(string expression, bool isSoft)
		{
			Expression = expression;
			IsSoft = isSoft;
		}

		public override void Execute(RenSharpCore core)
		{
			var ctx = core.Context;
			if(string.IsNullOrEmpty(Expression) == false)
			{
				string exp = ctx.InterpolateString(Expression);
				ctx.SetVariable("_return", ctx.Evaluate<object>(exp));
			}

			// Soft return won't pop last element
			if (IsSoft)
				ctx.TryPopState();
			else
				ctx.PopState();
		}
	}
}
