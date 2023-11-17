using System;
using System.Collections.Generic;
using System.Linq;
using RenSharp.Models;
using System.Text.RegularExpressions;
using RenSharp.Interfaces;
using RenSharp.Core.Expressions;

namespace RenSharp.Core
{
    public class RenSharpContext
    {
		internal RenSharpProgram Program { get; set; }
		internal PythonEvaluator PyEvaluator { get; set; }

		public MessageHistory MessageHistory { get; private set; }
		private Stack<StackFrame> CallStack { get; set; }
		internal Stack<Command> RollbackStack { get; private set; }
		internal StackFrame CurrentFrame { get; set; }

		internal Stack<int> LevelStack => CurrentFrame.LevelStack;
		internal int Level => LevelStack.Count + 1;

		public RenSharpContext()
		{
			CallStack = new Stack<StackFrame>();
			RollbackStack = new Stack<Command>();
			MessageHistory = new MessageHistory();

			PyEvaluator = new PythonEvaluator();
			CurrentFrame = new StackFrame();
		}

		internal void Goto(Command command)
		{
			// Если был вызван 'PushState' то стек сохранен в 'CallStack'
			Program.Goto(command.Line);
			RewriteLevelStack(command);
		}

		#region StackState
		public void PushState()
		{
			CurrentFrame.Line = Program.Current.Line;
			CallStack.Push(CurrentFrame);
			CurrentFrame = new StackFrame();
		}

		internal void PopState()
		{
			CurrentFrame = CallStack.Pop();
			int line = CurrentFrame.Line;
			Program.Goto(line + 1);
		}

		internal bool TryPopState()
		{
			try
			{
				PopState();
				return true;
			}
			catch
			{
				return false;
			}
		}

		internal void RewriteLevelStack(Command command)
		{
			LevelStack.Clear();
			int line = command.Line;
			int level = command.Level;

			Stack<int> levelStack = new Stack<int>();

			// После того как мы прыгнули на строчку мы идем назад
			// И ищем все команды, которые увеличивали табуляцию.
			// У этих команд вызываем метод Push, чтобы составить новый levelStack.
			while (levelStack.Count < command.Level - 1)
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

		internal void ClearRollback()
			=> RollbackStack.Clear();
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
				string value = Evaluate<object>(expression).ToString();

				line = line.Replace("{" + expression + "}", value);
			}

			if (notEscapedBrackets.IsMatch(line))
				throw new ArgumentException($"Wrong brackets count with line '{line}'");
			line = line.Replace("\\{", "{").Replace("\\}", "}");

			return line;
		}
		public T Evaluate<T>(string expression) => (T)PyEvaluator.Evaluate(expression);
		public void ExecutePython(IEnumerable<string> lines)
			=> PyEvaluator.Execute(lines);

		public void ExecutePython(string line)
			=> PyEvaluator.Execute(line);

		internal void SetVariable(string name, object value)
			=> PyEvaluator.SetVariable(name, value);
		
		internal dynamic GetVariable(string name)
			=> PyEvaluator.GetVariable(name);

		internal T GetVariable<T>(string name)
			=> PyEvaluator.GetVariable<T>(name);
		#endregion
	}
}
