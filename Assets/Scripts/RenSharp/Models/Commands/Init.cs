using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Init : Command
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
	}
}
