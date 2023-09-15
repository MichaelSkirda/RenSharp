using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

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
			string result = context.ExecuteExpression<object>(Expression).ToString();
			context.Variables[Name] = result;
		}
	}
}
