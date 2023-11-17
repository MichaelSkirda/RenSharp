using RenSharp.Core;
using System.Collections.Generic;

namespace RenSharp.Interfaces
{
	internal interface IPushable
	{
		// Метод Execute у Command решает надо ли испльзовать Push.
		// Метод Push решает какое значение надо пушить.
		bool Push(RenSharpContext ctx);
		bool Push(Stack<int> stack, RenSharpContext ctx);
	}
}
