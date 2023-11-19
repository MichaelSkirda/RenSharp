using RenSharp.Core;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
	public class Init : PushableCommand
	{
		public int Priority { get; set; }
		public bool IsPython { get; set; }

		public Init(int priority, bool isPython)
		{
			Priority = priority;
			IsPython = isPython;
		}

		public override void Execute(RenSharpCore core)
		{
			
		}

		public override void Push(Stack<int> stack, RenSharpContext ctx)
		{
			stack.Push(0);
		}

		public override bool TryPush(Stack<int> stack, RenSharpContext ctx)
		{
			// Do nothing
			// No push! Init must be ingnored in runtime
			// Works like 'if (False)'
			return false;
		}
	}
}
