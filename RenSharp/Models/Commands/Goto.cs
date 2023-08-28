using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Goto : Command
	{
		internal string LabelName { get; set; }

		internal override void Execute(RenSharpCore renSharpCore)
		{
			renSharpCore.GotoLabel(LabelName);
		}
	}
}
