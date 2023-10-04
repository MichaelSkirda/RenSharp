using System;
using System.Collections.Generic;
using System.Text;
using RenSharp.Core;

namespace RenSharp.Models.Commands
{
    public class Goto : Command
	{
		public string Label { get; set; }
		public bool Evaluate { get; set; }

		public Goto(string label, bool evaluate)
		{
			if (string.IsNullOrWhiteSpace(label))
				throw new ArgumentNullException("Goto label can not be null or empty");
			Label = label;
			Evaluate = evaluate;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext ctx)
		{
			string labelName = Label;
			if (Evaluate)
			{
				labelName = ctx.InterpolateString(Label);
				labelName = ctx.ExecuteExpression<string>(labelName);
			}
			renSharpCore.Goto(labelName);
		}
	}
}
