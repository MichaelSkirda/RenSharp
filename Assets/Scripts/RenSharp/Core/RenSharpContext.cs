using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Reflection;
using RenSharp.Models;
using System.Text.RegularExpressions;

namespace RenSharp.Core
{
    public class RenSharpContext
    {
		internal RenSharpProgram Program { get; set; }
		internal Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
		internal Stack<StackFrame> Stack { get; set; } = new Stack<StackFrame>();
		internal Stack<int> LevelStack => CurrentFrame.LevelStack;
		internal StackFrame CurrentFrame => Stack.Peek();
		internal int Level => LevelStack.Count + 1;

		private ExpressionContext FleeCtx { get; set; }

		public RenSharpContext()
		{
			FleeCtx = new ExpressionContext();
			FleeCtx.ParserOptions.DecimalSeparator = '.';
			FleeCtx.ParserOptions.FunctionArgumentSeparator = ',';
			FleeCtx.ParserOptions.RecreateParser();

			UpdateContext();

			var mainFrame = new StackFrame();
			Stack.Push(mainFrame);
		}

		public void PushStack()
		{
			CurrentFrame.Line = Program.Current.Line;
			var frame = new StackFrame();
			Stack.Push(frame);
		}

		internal StackFrame PopStack()
		{
			var oldFrame = Stack.Pop();
			Program.Goto(CurrentFrame.Line);
			return oldFrame;
		}

		public void UpdateContext()
		{
			List<string> variables = Variables.Keys.ToList();
			List<RenSharpMethod> methods = CallbackAttribute.Callbacks;

			foreach (string name in variables)
			{
				object value = GetValue(name);
				FleeCtx.Variables[name] = value;
			}
			foreach (RenSharpMethod method in methods)
			{
				FleeCtx.Imports.AddMethod(method.MethodInfo, method.Namespace);
			}

			FleeCtx.Variables["ctx"] = this;
		}

		internal object GetValue(string var)
		{
			object value = Variables[var];
			int num;
			bool isNumber = Int32.TryParse(value.ToString(), out num);

			if (isNumber)
				return num;

			return value;
		}
        internal T ExecuteExpression<T>(string expression)
        {
			UpdateContext();
			IGenericExpression<T> e = FleeCtx.CompileGeneric<T>(expression);

			T result = e.Evaluate();
			return result;
        }

		internal string MessageExecuteVars(string line)
		{
			// Value in brackets '{' '}' and brackets, except escaped brackets '\{' \}'
			Regex valueInBrackets = new Regex("(?<![\\\\])\\{(\\\\\\}|[^}])*(?<![\\\\])\\}");
			// Matches any not escaped bracket
			Regex notEscapedBrackets = new Regex("(?<![\\\\])\\{|(?<![\\\\])\\}");

			List<string> expressions = valueInBrackets.Matches(line)
				.Select(x => x.Value.Substring(1, x.Value.Length - 2))
				.Distinct()
				.ToList();

			if (expressions.Any(x => notEscapedBrackets.IsMatch(x)))
				throw new ArgumentException($"Wrong brackets count with line '{line}'");

			foreach(string expression in expressions)
			{
				string value = ExecuteExpression<object>(expression).ToString();

				line = line.Replace("{" + expression + "}", value);
			}

			if (notEscapedBrackets.IsMatch(line))
				throw new ArgumentException($"Wrong brackets count with line '{line}'");

			line = line.Replace("\\{", "{").Replace("\\}", "}");

			return line;
		}
    }
}
