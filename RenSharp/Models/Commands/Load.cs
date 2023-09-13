using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Load : Command
	{
		internal string Path { get; set; }

		public Load(string path)
		{
			Path = path;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			throw new NotImplementedException("You can not execute load command!");
		}
	}
}
