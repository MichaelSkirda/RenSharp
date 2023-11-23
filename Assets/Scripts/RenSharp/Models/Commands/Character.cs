using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
    public class Character
	{
		public Attributes Attributes { get; set; }

		public Character(IEnumerable<string> attributes = null)
		{
			Attributes = new Attributes(attributes);
		}

		public Character(string name)
		{
			Attributes = new Attributes();
			Attributes.AddAttribute("name", name, rewrite: true);
		}
	}
}
