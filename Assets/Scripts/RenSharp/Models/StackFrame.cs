using System.Collections.Generic;

namespace RenSharp.Models
{
	public class StackFrame
	{
		internal Stack<int> LevelStack = new Stack<int>();
		internal int Line { get; set; }
	}
}
