using System;
using System.Collections.Generic;
using System.Text;
using RenSharp.Core;

namespace RenSharp.Models.Commands
{
    public class Goto : Command
	{
		public string LabelName { get; set; }

		public Goto(string labelName)
		{
			LabelName = labelName;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			renSharpCore.Goto(LabelName);
		}
	}
}
