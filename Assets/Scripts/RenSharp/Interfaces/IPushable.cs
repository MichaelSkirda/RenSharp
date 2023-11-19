using RenSharp.Core;
using System.Collections.Generic;

namespace RenSharp.Interfaces
{
	internal interface IPushable
	{
		// Push всегда пушит значение.
		void Push(RenSharpContext ctx);
		void Push(Stack<int> stack, RenSharpContext ctx);

		// TryPush возвращает true если запушил, иначе false.
		bool TryPush(RenSharpContext ctx);
		bool TryPush(Stack<int> stack, RenSharpContext ctx);
	}
}
