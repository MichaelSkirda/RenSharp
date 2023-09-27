using RenSharp.Core;
using RenSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Init : Command, IPushable
	{
		public int Priority { get; set; }

		public Init(int priority)
		{
			if (priority < -999 || priority > 999)
				throw new ArgumentException("Priority of init can not be higher 999 and lower -999.");
			Priority = priority;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			// Do nothing
			// No push! Init must be ingnored in runtime
		}

		public void Push(Stack<int> stack, RenSharpContext ctx) => stack.Push(0);
	}
}
