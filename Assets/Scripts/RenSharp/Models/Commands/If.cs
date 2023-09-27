using Flee.PublicTypes;
using RenSharp.Core;
using RenSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			bool result = ctx.ExecuteExpression<bool>(Expression);
			if (result)
				Push(ctx.LevelStack, ctx);
		}

		public void Push(Stack<int> stack, RenSharpContext ctx) => stack.Push(EndIfLine);
		
	}
}
