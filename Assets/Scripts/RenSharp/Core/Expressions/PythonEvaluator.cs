﻿using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenSharp.Core.Expressions
{
	internal class PythonEvaluator
	{
		private static ScriptEngine _engine;
		private static ScriptEngine Engine
		{
			get
			{
				if(_engine == null)
					_engine = Python.CreateEngine();
				return _engine;
			}
		}
		private ScriptScope Scope { get; set; }

		internal PythonEvaluator()
		{
			var initializer = new PythonInitializer(Engine);
			Scope = initializer.Scope;
		}

		internal void RecreateScope() => Scope = Engine.CreateScope();
		internal void SetScope(ScriptScope scope)
		{
			if (scope == null)
				throw new ArgumentNullException("Нельзя установить 'null' как scope IronPython.");
			Scope = scope;
		}
		internal ScriptScope CopyScope()
		{
			IEnumerable<KeyValuePair<string, dynamic>> keyValues = Scope.GetItems();
			Dictionary<string, object> variables = keyValues
				.Where(kv => kv.Key.StartsWith("__") == false)
				.GroupBy(kv => kv.Key)
				.Select(kvs => kvs.First())
				.ToDictionary(x => x.Key, x => x.Value);
			return Engine.CreateScope(variables);
		}

		internal void Execute(IEnumerable<string> lines)
		{
			string code = lines.ToPythonCode();
			Execute(code);
		}

		internal void Execute(string code)
			=> Engine.Execute(code, Scope);

		internal object Evaluate(string code)
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

		internal void SetVariable(string name, object value)
		{
			Scope.SetVariable(name, value: value);
		}

		internal object GetVariable(string key)
			=> Scope.GetVariable(key);

		internal T GetVariable<T>(string key)
			=> Scope.GetVariable<T>(key);

		internal Dictionary<string, object> GetVariables()
		{
			IEnumerable<string> keys = Scope.GetVariableNames();
			var variables = new Dictionary<string, object>();

			foreach(string key in keys)
			{
				variables[key] = Scope.GetVariable(key);
			}

			return variables;
		}
	}
}
