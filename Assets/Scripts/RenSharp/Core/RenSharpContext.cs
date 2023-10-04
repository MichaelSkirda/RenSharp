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
		internal Dictionary<string, object> SystemVariables { get; set; } = new Dictionary<string, object>();
		internal Stack<StackFrame> Stack { get; set; } = new Stack<StackFrame>();
		private PythonEvaluator PyEvaluator { get; set; }

		internal Stack<int> LevelStack => CurrentFrame.LevelStack;
		internal int Level => LevelStack.Count + 1;
		internal StackFrame CurrentFrame => Stack.Peek();
		public RenSharpContext()
		{
			PyEvaluator = new PythonEvaluator();

			var mainFrame = new StackFrame();
			Stack.Push(mainFrame);
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
			_ = Stack.Pop();
			int line = CurrentFrame.Line;
			Program.Goto(line + 1);
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
						throw new ArgumentException($"Command '{cmd.GetType()}' not implement IPushable but pushes stacks.");
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
		{
			var keyValues = SystemVariables.ToList();
			PyEvaluator.SetVariables(keyValues);
			PyEvaluator.Execute(lines);
		}
		#endregion
	}
}
