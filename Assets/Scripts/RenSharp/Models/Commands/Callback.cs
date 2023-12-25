using RenSharp.Core;
using System;

namespace RenSharp.Models.Commands
{
	public class Callback : Command
	{
		public string Expression { get; set; }

		[Obsolete("Replaced with IronPython and PyImport attribute")]
		public Callback(string expression)
		{
			throw new NotImplementedException();
			Expression = expression;
		}

		public override void Execute(RenSharpCore core)
		{
			_ = core.Context.Evaluate<object>(Expression);
		}
	}
}
