using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RenSharp.Core
{
    public class RenSharpContext
    {
		internal Stack<int> Stack = new Stack<int>();

		/// <summary>
		/// Tabulation level
		/// </summary>
        internal int Level => Stack.Count + 1;
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
			//[a-zA-Z]+\([^\)]*\)
			List<string> names = StringManipulator.GetVars(expression)
				.Distinct()
				.ToList();
			List<MethodInfo> methods = CallbackAttribute.MethodsList;

			var exp = new ExpressionContext();

			foreach (string name in names)
			{
				object value = GetValue(name);
				exp.Variables[name] = value;
			}
			foreach (MethodInfo method in methods)
			{
				exp.Imports.AddMethod(method, method.DeclaringType.Namespace);
			}

			IGenericExpression<T> e = exp.CompileGeneric<T>(expression);

			T result = e.Evaluate();
			return result;
		}

		internal string MessageExecuteVars(string line, RenSharpContext context)
		{
			while (true)
			{
				if (line.Contains("{") == false || line.Contains("}") == false)
					break;

				string expression = line.GetStringBetween("{", "}");
				string value = context.ExecuteExpression<object>(expression).ToString();

				line = line.Replace("{" + expression + "}", value);
			}
			return line;
		}
    }
}
