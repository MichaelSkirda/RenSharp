using RenSharp.Core;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
	public class While : PushableCommand
	{
		public string Expression { get; set; }
		public While(string expression)
		{
			Expression = expression;
		}

		public override void Execute(RenSharpCore core)
		{
			
		}

		public override void Push(Stack<int> stack, RenSharpContext ctx)
		{
			bool result = ctx.Evaluate<bool>(Expression);
			if (result)
				stack.Push(Line);
			else
				stack.Push(0);
		}

		public override bool TryPush(Stack<int> stack, RenSharpContext ctx)
		{
			bool result = ctx.Evaluate<bool>(Expression);
			if (result)
			{
				stack.Push(Line);
				return true;
			}
			return false;
		}

	}
}
