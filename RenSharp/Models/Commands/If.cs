using Flee.PublicTypes;
using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenSharp.Models.Commands
{
	public class If : Command
	{
		public string Expression { get; set; }
		public int EndIfLine { get; set; }
		public bool IsRoot { get; set; }

		public If(string expression, bool isRoot)
		{
			Expression = expression;
			IsRoot = isRoot;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			bool result = context.ExecuteExpression<bool>(Expression);
			if (result)
				context.Stack.Push(EndIfLine);
		}
	}
}
