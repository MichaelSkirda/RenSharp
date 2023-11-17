using RenSharp.Core;
using RenSharp.Interfaces;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
	public class Repeat : Command, IPushable
	{
		public string Expression { get; set; }
		public int? Times { get; private set; } = null;
		private int Repeated { get; set; } = 0;

		public Repeat(string expression)
		{
			Expression = expression;
		}

		public override void Execute(RenSharpCore core)
		{

		}

		private void Reset()
		{
			Times = null;
			Repeated = 0;
		}
		public bool Push(RenSharpContext ctx)
		{
			if (Times == null)
				Times = ctx.Evaluate<int>(Expression);

			if (Repeated < Times.Value)
			{
				Repeated++;
				ctx.LevelStack.Push(Line);
				return true;
			}
			else
			{
				Reset();
			}
			return false;
		}
	}
}
