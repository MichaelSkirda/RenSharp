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

		public If(string expression)
		{
			Expression = expression;
		}

		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
			// 'x + 12 == y || x == "hello"' -> ['x', 'y']
			List<string> vars = StringManipulator.GetVars(Expression);

			ExpressionContext exp = new ExpressionContext();
			
			foreach(string var in vars)
			{
				bool isNumber = Int32.TryParse(var, out _);
				if (isNumber)
					continue;

				string value = context.Variables[var];
				int num;
				isNumber = Int32.TryParse(value, out num);
				if(isNumber)
					exp.Variables[var] = num;
				else
					exp.Variables[var] = value;
			}

			IGenericExpression<bool> e = exp.CompileGeneric<bool>(Expression);

			bool result = e.Evaluate();
			if (result)
				context.Level++;
		}
	}
}
