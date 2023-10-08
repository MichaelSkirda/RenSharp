using Microsoft.Scripting.Hosting;
using System.Collections.Generic;

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
			Execute(code);
		}

		internal void Execute(string code)
			=> Engine.Execute(code, Scope);

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
				SetVariable(keyValue.Key, keyValue.Value);
			}
		}

		internal void SetVariable(string name, dynamic value)
			=> Scope.SetVariable(name, value);

		internal dynamic GetVariable(string key)
			=> Scope.GetVariable(key);

		internal T GetVariable<T>(string key)
			=> Scope.GetVariable<T>(key);
	}
}
