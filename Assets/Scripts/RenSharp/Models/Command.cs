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
		internal abstract void Execute(RenSharpCore core);
		internal bool IsNot<T>() where T : Command => !(this is T);
		internal bool Is<T>() where T : Command => this is T;
	}
}
