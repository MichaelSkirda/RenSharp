using System.Collections.Generic;
using RenSharp.Core;

namespace RenSharp.Models.Commands
{
    public class Label : PushableCommand
	{
		public string Name { get; set; }

		public Label(string name)
		{
			Name = name;
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
			stack.Push(0);
			return true;
		}
	}
}
