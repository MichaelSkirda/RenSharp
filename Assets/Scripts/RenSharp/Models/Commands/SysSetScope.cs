using Microsoft.Scripting.Hosting;
using RenSharp.Core;

namespace RenSharp.Models.Commands
{
	public class SysSetScope : Command
	{
		private ScriptScope Scope { get; set; }

		public SysSetScope(ScriptScope scope)
		{
			Scope = scope;
		}

		public override void Execute(RenSharpCore core)
		{
			core.Context.PyEvaluator.SetScope(Scope);
		}
	}
}
