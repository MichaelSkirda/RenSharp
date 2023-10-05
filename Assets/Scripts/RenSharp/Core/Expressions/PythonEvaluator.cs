using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RenSharp.Core.Expressions
{
	internal class PythonEvaluator
	{
		private ScriptEngine Engine { get; set; }
		private ScriptScope Scope { get; set; }

		internal PythonEvaluator()
		{
			var initializer = new PythonInitializer();
			Engine = initializer.Engine;
			Scope = initializer.Scope;
		}

		internal void RecreateScope() => Scope = Engine.CreateScope();
		internal void Execute(IEnumerable<string> lines)
		{
			string code = lines.ToPythonCode();
			Engine.Execute(code, Scope);
		}

		internal dynamic Evaluate(string code)
		{
			code = $"_evaluation_ = ({code})";
			Engine.Execute(code, Scope);
			return Scope.GetVariable("_evaluation_");
		}

		internal void SetVariables(IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			foreach (var keyValue in keyValues)
			{
				SetVariable(keyValue);
			}
		}

		internal void SetVariable(KeyValuePair<string, object> keyValue)
			=> Scope.SetVariable(keyValue.Key, keyValue.Value);

		internal object GetVariable(string key)
			=> Scope.GetVariable(key);
	}
}
