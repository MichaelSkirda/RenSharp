using System.Collections.Generic;

namespace RenSharp.Models
{
	internal class StackFrame
	{
		internal Stack<int> LevelStack = new Stack<int>();
		internal int Line { get; set; }
	}
}
