using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	public class Set : Command
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public Set(string name, string value)
		{
			Name = name;
			Value = value;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			context.Variables[Name] = Value;
		}
	}
}
