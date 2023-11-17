using RenSharp.Core;
using RenSharp.Interfaces;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
	public class Init : Command, IPushable
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
			// Do nothing
			// No push! Init must be ingnored in runtime
			// Works like 'if (False)'
		}

		public bool Push(RenSharpContext ctx) => false;

		public bool Push(Stack<int> stack, RenSharpContext ctx) => false;
	}
}
