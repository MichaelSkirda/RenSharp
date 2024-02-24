using Microsoft.Scripting.Hosting;
using RenSharp.Core;
using RenSharp.Core.Expressions;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{

    public class SysSetScope : Command
	{
		private ScriptScope Scope { get; set; }
		public Dictionary<string, object> variables => PythonEvaluator.GetVariables(Scope);

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
