using System.Collections.Generic;

namespace RenSharp.Models
{
	public class StackFrame
	{
		public Stack<int> LevelStack { get; set; } = new Stack<int>();
		public int Line { get; set; }
	}
}
