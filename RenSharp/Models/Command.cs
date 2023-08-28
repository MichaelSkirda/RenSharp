using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models
{
	internal abstract class Command
	{
		internal int Level { get; set; }
		internal abstract void Execute(RenSharpCore renSharpCore);
	}
}
