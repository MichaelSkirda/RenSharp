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

namespace RenSharp.Core
{
    public class RenSharpContext
    {
		internal RenSharpProgram Program { get; set; }
		internal Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
		internal Stack<StackFrame> Stack { get; set; } = new Stack<StackFrame>();
		private ExpressionContext FleeCtx { get; set; }
		private ScriptEngine Engine { get; set; }
		private ScriptScope Scope { get; set; }

		internal Stack<int> LevelStack => CurrentFrame.LevelStack;
		internal int Level => LevelStack.Count + 1;
		internal StackFrame CurrentFrame => Stack.Peek();
		public RenSharpContext()
		{
			InitPython();

			UpdateExpressionContext();

			var mainFrame = new StackFrame();
			Stack.Push(mainFrame);
		}

		private void InitPython()
		{
			Engine = Python.CreateEngine();
			Scope = Engine.CreateScope();

			dynamic exp = new System.Dynamic.ExpandoObject();


			List<ImportMethod> methods = PyImportAttribute.MethodImports;
			List<ImportType> types = PyImportAttribute.TypeImports;

			string loadAssemblies = string.Join("\n", methods
				.Select(x => $"clr.AddReference(\"{x.MethodInfo.DeclaringType.Assembly.FullName}\")")
				.Distinct());

			string importMethods = string.Join("\n",
			methods.Select(x => string.IsNullOrEmpty(x.Name)
			? $"from {x.MethodInfo.DeclaringType.Namespace}.{x.MethodInfo.DeclaringType.Name} import {x.MethodInfo.Name}"
			: $"from {x.MethodInfo.DeclaringType.Namespace}.{x.MethodInfo.DeclaringType.Name} import {x.MethodInfo.Name} as {x.Name}"
			));

			string importTypes = string.Join("\n",
			types.Select(x => string.IsNullOrWhiteSpace(x.Name)
			? $"import {x.Type.Namespace}.{x.Type.Name}"
			: $"import {x.Type.Namespace}.{x.Type.Name} as {x.Name}"
			));

			// All public static methods that's contains PyImportAttribute
			List<string> renameMethods = new List<string>();


			foreach (ImportType import in types)
			{
				List<MethodInfo> toRename = import.Type
					.GetMethods(BindingFlags.Public | BindingFlags.Static)
					.Where(x => x.IsDefined(typeof(PyImportAttribute)))
					.ToList();

				foreach (MethodInfo method in toRename)
				{
					string methodName = method.GetCustomAttribute<PyImportAttribute>().Name;
					if (string.IsNullOrWhiteSpace(methodName))
						throw new Exception("Если у класса есть PyImport, то методы с PyImport должны объявлять имя.");

					if (string.IsNullOrEmpty(import.Name))
					{
						renameMethods.Add($"{import.Type.Name}.{methodName} = {import.Type.Name}.{method.Name}");
						renameMethods.Add($"del {import.Type.Name}.{method.Name}");
					}
					else
					{
						renameMethods.Add($"{import.Name}.{methodName} = {import.Name}.{method.Name}");
						renameMethods.Add($"del {import.Name}.{method.Name}");
					}
				}
			}

			Engine.Execute("import clr", Scope);
			Engine.Execute(loadAssemblies, Scope);
			Engine.Execute(importMethods, Scope);
			Engine.Execute(importTypes, Scope);
			Engine.Execute(string.Join("\n", renameMethods), Scope);
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
		public void UpdateExpressionContext()
		{
			FleeCtx = new ExpressionContext();
			FleeCtx.ParserOptions.DecimalSeparator = '.';
			FleeCtx.ParserOptions.FunctionArgumentSeparator = ',';
			FleeCtx.ParserOptions.RecreateParser();

			List<string> variables = Scope.GetVariableNames().ToList();
			List<ImportMethod> methods = PyImportAttribute.MethodImports;

			foreach (string name in variables)
			{
				object value = Scope.GetVariable(name);
				if (value == null)
					continue;
				FleeCtx.Variables[name] = value;
			}
			foreach (ImportMethod method in methods)
			{
				FleeCtx.Imports.AddMethod(method.MethodInfo, method.Name);
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
		internal T ExecuteExpression<T>(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
				throw new ArgumentException("Expression can not be null or whitespace.");
			UpdateExpressionContext();
			IGenericExpression<T> e = FleeCtx.CompileGeneric<T>(expression);

			T result = e.Evaluate();
			return result;
		}

		internal dynamic ExecuteExpression(string expression)
		{
			UpdateExpressionContext();
			IDynamicExpression e = FleeCtx.CompileDynamic(expression);

			dynamic result = e.Evaluate();
			return result;
		}

		internal void UpdatePythonContext()
		{
			List<string> variables = Variables.Keys.ToList();

			foreach (string name in variables)
			{
				object value = GetValue(name);
				Scope.SetVariable(name, value);
			}
			
			FleeCtx.Variables["ctx"] = this;
		}

		internal void ExecutePython(IEnumerable<string> lines)
		{
			UpdatePythonContext();
			string code = string.Join("\n", lines);
			Engine.Execute(code, Scope);
		}
		#endregion
	}
}
