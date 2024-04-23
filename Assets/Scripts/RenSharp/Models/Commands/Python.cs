using Microsoft.Scripting.Hosting;
using RenSharp.Core;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
	// Contains whole 'python:' block
	public class Python : Command, IRollbackable
	{
		public List<string> Commands { get; set; } = new List<string>();
		public Python() { }

		public override void Execute(RenSharpCore core)
		{
			core.Context.ExecutePython(Commands);
		}

		public Command Rollback(RenSharpCore core)
		{
			ScriptScope scope = core.Context.PyEvaluator.CopyScope();
			if (scope == null)
				return null;

			Command command = new SysSetScope(scope);
			command.SetPosition(this);

			return command;
		}

	}
}
