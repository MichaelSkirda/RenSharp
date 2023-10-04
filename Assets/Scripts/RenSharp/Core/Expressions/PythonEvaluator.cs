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
			InitPython();
		}

		private void InitPython()
		{
			Engine = Python.CreateEngine();
			Scope = Engine.CreateScope();

			string renSharpWrapper = @"
class RenSharpWrapper():
	def __setattr__(self, name, value):
		self.__dict__[name] = value
";
			Engine.Execute(renSharpWrapper, Scope);


			List<ImportMethod> methods = PyImportAttribute.MethodImports;

			string loadAssemblies = string.Join("\n", methods
				.Select(x => $"clr.AddReference(\"{x.MethodInfo.DeclaringType.Assembly.FullName}\")")
				.Distinct());

			Engine.Execute("import clr", Scope);
			Engine.Execute(loadAssemblies, Scope);

			string importMethods =
			methods.Select(x => {
				var m = x.MethodInfo;
				bool hasName = !string.IsNullOrEmpty(x.Name);
				if (hasName)
					return $"from {m.DeclaringType.Namespace}.{m.DeclaringType.Name} import {m.Name} as {x.Name}";
				else
					return $"from {m.DeclaringType.Namespace}.{m.DeclaringType.Name} import {m.Name}";
			}).ToPythonCode();
			Engine.Execute(importMethods, Scope);
			 

			 
			List<ImportType> types = PyImportAttribute.TypeImports;
			foreach (ImportType import in types)
			{
				List<MethodInfo> toRename = import.Type
					.GetMethods(BindingFlags.Public | BindingFlags.Static)
					.Where(x => !x.IsSpecialName)
					.ToList(); 


				string typeName = import.Name;
				if(string.IsNullOrEmpty(typeName))
					typeName = import.Type.Name;

				PropertyInfo[] propertiesImports = import.Type
					.GetProperties(BindingFlags.Public | BindingFlags.Static);


				Engine.Execute($"import {import.Type.Namespace}.{import.Type.Name} as RSImport", Scope);
				Engine.Execute($"{typeName} = RenSharpWrapper()", Scope);

				foreach (MethodInfo method in toRename)
				{
					string methodName = method.GetCustomAttribute<PyImportAttribute>()?.Name;
					if (string.IsNullOrWhiteSpace(methodName)) 
						methodName = method.Name;

					Engine.Execute($"{typeName}.{methodName} = RSImport.{method.Name}", Scope);
				}

				foreach(PropertyInfo property in propertiesImports)
				{
					Engine.Execute($"{typeName}.{property.Name} = RSImport.{property.Name}", Scope);
				}

				Engine.Execute($"del RSImport", Scope);
			}
		}

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
	}
}
