using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core
{
	internal static class StackExtension
	{
		internal static Stack<T> ReverseStack<T>(this Stack<T> stack)
		{
			// IDK why but it is reverse stack
			Stack<T> clone = new Stack<T>(stack);
			return clone;
		}
	}
}
