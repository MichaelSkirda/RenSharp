using RenSharp.Core;
using RenSharp.Interfaces;
using System.Collections.Generic;

namespace RenSharp.Models.Commands
{
	public class If : Command, IPushable
	{
		public string Expression { get; set; }
		public int EndIfLine { get; set; }
		public bool IsRoot { get; set; }

		public If(string expression, bool isRoot)
		{
			Expression = expression;
			IsRoot = isRoot;
		}

		public override void Execute(RenSharpCore core)
		{

		}

		public bool Push(RenSharpContext ctx)
			=> Push(ctx.LevelStack, ctx);

		public bool Push(Stack<int> stack, RenSharpContext ctx)
		{
			bool result = ctx.Evaluate<bool>(Expression);
			if (result)
			{
				stack.Push(EndIfLine);
				return true;
			}
			return false;
		}
	}
}
