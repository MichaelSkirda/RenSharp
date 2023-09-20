using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Callback : Command
	{
		public string FunctionName { get; set; }
		public string[] Args { get; set; }

		public Callback(string funcName, string[] args)
		{
			FunctionName = funcName;
			Args = args;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			renSharpCore.Callback(FunctionName, renSharpCore, Args);
		}
	}
}
