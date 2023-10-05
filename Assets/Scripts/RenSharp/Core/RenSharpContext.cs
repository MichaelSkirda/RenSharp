using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RenSharp.Models;
using System.Text.RegularExpressions;
using RenSharp.Interfaces;
using IDynamicExpression = Flee.PublicTypes.IDynamicExpression;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using RenSharp.Core.Expressions;

namespace RenSharp.Core
{
    public class RenSharpContext
    {
		internal RenSharpProgram Program { get; set; }
		private Stack<StackFrame> Stack { get; set; }
		private PythonEvaluator PyEvaluator { get; set; }

		internal Stack<int> LevelStack => CurrentFrame.LevelStack;
		internal int Level => LevelStack.Count + 1;
		internal StackFrame CurrentFrame { get; set; }
		public RenSharpContext()
		{
			Stack = new Stack<StackFrame>();
			PyEvaluator = new PythonEvaluator();
			CurrentFrame = new StackFrame();
		}

		internal void Goto(Command command)
		{
			Program.Goto(command.Line);
			RewriteStack(command);
		}

		#region StackState
		public void PushState()
		{
			CurrentFrame.Line = Program.Current.Line;
			var frame = new StackFrame();
			Stack.Push(frame);
		}

		internal void PopState()
		{
			CurrentFrame = Stack.Pop();
			int line = CurrentFrame.Line;
			Program.Goto(line + 1);
		}

		internal bool TryPopState()
		{
			if (Stack.Count > 0)
				return false;
			PopState();
			return true;
		}

		internal void RewriteStack(Command command)
		{
			LevelStack.Clear();
			int line = command.Line;
			int level = command.Level;

			Stack<int> levelStack = new Stack<int>();

			while (levelStack.Count + 1 < command.Level)
			{
				line--;
				Command cmd = Program[line];
				if (cmd.Level < level)
				{
					level--;
					IPushable pushableCmd = cmd as IPushable;
					if (pushableCmd == null)
						throw new ArgumentException($"Команда '{cmd.GetType()}' не реализует IPushable, но использует табуляцию.");
					pushableCmd.Push(levelStack, this);
				}
			}
			CurrentFrame.LevelStack = levelStack.ReverseStack();
		}
		#endregion

		#region EXPRESSIONS
		internal string InterpolateString(string line)
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
		internal T ExecuteExpression<T>(string expression) => PyEvaluator.Evaluate(expression);
		internal void ExecutePython(IEnumerable<string> lines)
			=> PyEvaluator.Execute(lines);
		
		internal void SetVariable(string name, object value)
		{
			var keyValue = new KeyValuePair<string, object>(name, value);
			PyEvaluator.SetVariable(keyValue);
		}
		internal void GetVariable(string name)
			=> PyEvaluator.GetVariable(name);
		#endregion
	}
}
