using Microsoft.Scripting.Hosting;
using RenSharp.Core;
using System;

namespace RenSharp.Models.Commands
{
	internal class SetScope : Command
	{
		public ScriptScope Scope { get; set; }

		public SetScope(ScriptScope scope)
		{
			Scope = scope;
		}

		public override void Execute(RenSharpCore core)
		{
			throw new NotImplementedException();
		}
	}
}
