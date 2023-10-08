using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Callback : Command
	{
		internal string Expression { get; set; }

		public Callback(string expression)
		{
			Expression = expression;
		}

		internal override void Execute(RenSharpCore core)
		{
			_ = core.Context.Evaluate<object>(Expression);
		}
	}
}
