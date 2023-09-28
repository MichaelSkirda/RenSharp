using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RenSharp.Models.Commands
{
	public class Set : Command
	{
		public string Name { get; set; }
		public string Expression { get; set; }

		public Set(string name, string expression)
		{
			Name = name;
			Expression = expression;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			Regex getVarName = new Regex("^[a-zA-Z]+[a-zA-Z0-9]*(?=\\.|$)");
			var matches = getVarName.Matches(Name);
			if (matches.Count != 1)
				throw new ArgumentException("Variable name can not be null or contains new line characters.");
			string varName = matches[0].Value;

			dynamic result = context.ExecuteExpression(Expression);
			context.Variables[varName] = result;
		}
	}
}
