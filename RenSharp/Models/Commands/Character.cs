using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	public class Character : Command
	{
		public string Name { get; set; }
		public List<string> Styles { get; set; }

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
