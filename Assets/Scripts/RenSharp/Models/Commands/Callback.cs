using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	public class Callback : Command
	{
		public string Expression { get; set; }

		public Callback(string expression)
		{
			Expression = expression;
		}

		public override void Execute(RenSharpCore core)
		{
			_ = core.Context.Evaluate<object>(Expression);
		}
	}
}
