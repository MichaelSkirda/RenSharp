using System;
using System.Collections.Generic;
using System.Text;
using RenSharp.Core;

namespace RenSharp.Models.Commands
{
    public class Label : Command
	{
		public string Name { get; set; }

		public Label(string name)
		{
			Name = name;
		}

		internal override void Execute(RenSharpCore renSharpCore)
		{

		}
	}
}
