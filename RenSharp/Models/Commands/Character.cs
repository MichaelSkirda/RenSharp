using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Character : Command
	{
		internal string Name { get; set; }
		internal List<string> Styles { get; set; }

		public Character(string name, List<string> styles = null)
		{
			Name = name;
			Styles = styles;
		}

		internal override void Execute(RenSharpCore renSharpCore)
		{

		}
	}
}
