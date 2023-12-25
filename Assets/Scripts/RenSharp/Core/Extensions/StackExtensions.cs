using System;
using System.Collections.Generic;

namespace RenSharp.Core.Extensions
{
	internal static class StackExtensions
	{
		internal static Stack<T> Clone<T>(this Stack<T> original)
		{
			var arr = new T[original.Count];
			original.CopyTo(arr, 0);
			Array.Reverse(arr);
			return new Stack<T>(arr);
		}
	}
}
