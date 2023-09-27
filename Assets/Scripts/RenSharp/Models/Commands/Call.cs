using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Call : Command
	{
		public string LabelName { get; set; }

		public Call(string labelName)
		{
			LabelName = labelName;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			ctx.PushState();
			renSharpCore.Goto(LabelName);
		}
	}
}
