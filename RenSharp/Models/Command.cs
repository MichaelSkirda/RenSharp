using System;
using System.Collections.Generic;
using System.Text;
using RenSharp.Core;

namespace RenSharp.Models
{
    public abstract class Command
	{
		public int Level { get; set; }
		public int Line { get; set; }
		internal abstract void Execute(RenSharpCore renSharpCore);
	}
}
