using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace RenSharp.Core
{
    public class RenSharpContext
    {
        internal int Level { get; set; } = 1; // Tabulation level
        internal Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

		internal object GetValue(string var)
		{
			string value = Variables[var];
			int num;
			bool isNumber = Int32.TryParse(value, out num);

			if (isNumber)
				return num;

			return value;
		}
        internal T ExecuteExpression<T>(string expression)
        {
			List<string> names = StringManipulator.GetVars(expression);
			var exp = new ExpressionContext();

			foreach (string name in names)
			{
				object value = GetValue(name);
				exp.Variables[name] = value;
			}

			IGenericExpression<T> e = exp.CompileGeneric<T>(expression);

			T result = e.Evaluate();
			return result;
		}

		internal string ReplaceVars(string line)
		{
			while (true)
			{
				if (line.Contains("{") == false || line.Contains("}") == false)
					break;

				string name = line.GetStringBetween("{", "}");
				string value = Variables[name];

				line = line.Replace("{" + name + "}", value);
			}
			return line;
		}
    }
}
