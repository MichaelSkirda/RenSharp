using System.Collections.Generic;
using RenSharp.Core;

namespace RenSharp.Models.Commands
{
    public class Character : Command
	{
		public string Name { get; set; }
		public Attributes Attributes { get; set; }

		public Character(string name, IEnumerable<string> attributes = null)
		{
			Name = name;
			Attributes = new Attributes(attributes);
		}

		public override void Execute(RenSharpCore core)
		{

		}
	}
}
