using RenSharp.Core;
using RenSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	// Contains whole 'python:' block
	internal class Python : Command
	{
		public List<string> Commands { get; set; } = new List<string>();
		public Python() { }

		internal override void Execute(RenSharpCore core, RenSharpContext ctx)
		{
			ctx.ExecutePython(Commands);
		}

	}
}
