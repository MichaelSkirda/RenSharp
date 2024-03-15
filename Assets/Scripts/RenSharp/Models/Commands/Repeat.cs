using RenSharp.Core;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
	public class Repeat : CommandPushable
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

		public override void Push(Stack<int> stack, RenSharpContext ctx)
		{
			if (Times == null)
				Times = ctx.Evaluate<int>(Expression);

			Repeated++;

			if (Repeated < Times.Value)
				stack.Push(Line);
			else
				stack.Push(0);
		}

		public override bool TryPush(Stack<int> stack, RenSharpContext ctx)
		{
			if (Times == null)
				Times = ctx.Evaluate<int>(Expression);

			if(Repeated < Times.Value)
			{
				Repeated++;
				stack.Push(Line);
				return true;
			}
			return false;
		}

	}
}
