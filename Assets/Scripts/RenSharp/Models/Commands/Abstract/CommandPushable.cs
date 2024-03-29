﻿using RenSharp.Core;
using RenSharp.Interfaces;
using System.Collections.Generic;

namespace RenSharp.Models
{
	public abstract class CommandPushable : Command, IPushable
	{
		public void Push(RenSharpContext ctx)
			=> Push(ctx.LevelStack, ctx);
		public abstract void Push(Stack<int> stack, RenSharpContext ctx);

		public bool TryPush(RenSharpContext ctx)
			=> TryPush(ctx.LevelStack, ctx);
		public abstract bool TryPush(Stack<int> stack, RenSharpContext ctx);
	}
}
