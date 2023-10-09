using RenSharp.Core;
using RenSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	// Contains whole 'python:' block
	public class Python : Command
	{
		public List<string> Commands { get; set; } = new List<string>();
		public Python() { }

		public override void Execute(RenSharpCore core)
		{
			core.Context.ExecutePython(Commands);
		}

	}
}
