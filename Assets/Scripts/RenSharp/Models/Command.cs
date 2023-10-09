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
		public int SourceLine { get; set; }
		public abstract void Execute(RenSharpCore core);
		public bool IsNot<T>() where T : Command => !(this is T);
		public bool Is<T>() where T : Command => this is T;
	}
}
