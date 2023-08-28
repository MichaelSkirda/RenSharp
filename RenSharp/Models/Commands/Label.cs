using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Label : Command
	{
		internal string Name { get; set; }
		internal int Line { get; set; }

		public Label(string name)
		{
			Name = name;
		}

		internal override void Execute(RenSharpCore renSharpCore)
		{

		}
	}
}
