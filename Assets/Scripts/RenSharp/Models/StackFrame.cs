using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models
{
	internal class StackFrame
	{
		internal Stack<int> LevelStack = new Stack<int>();
		internal int Level => LevelStack.Count + 1;

	}
}
