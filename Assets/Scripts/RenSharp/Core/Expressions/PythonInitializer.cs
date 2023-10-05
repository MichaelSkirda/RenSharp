﻿using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using static IronPython.Modules._ast;

namespace RenSharp.Core.Expressions
{
	internal class PythonInitializer
	{
		internal ScriptEngine Engine { get; set; }
		internal ScriptScope Scope { get; set; }

		internal PythonInitializer()
		{
			Engine = Python.CreateEngine();
			Scope = Engine.CreateScope();
			InitPython();
		}

		private void InitPython()
		{
			ImportSystemTypes();
			AddClrReferences();

			List<ImportMethod> methods = PyImportAttribute.MethodImports;
			List<ImportType> types = PyImportAttribute.TypeImports;

			ImportMethods(methods);
			ImportTypes(types);
		}

		private void ImportMethods(IEnumerable<ImportMethod> methods)
		{
			string importMethods =
			methods.Select(x => {
				MethodInfo method = x.MethodInfo;
				Type type = x.MethodInfo.DeclaringType;
				return $"from {type.Namespace}.{type.Name} import {method.Name} as {x.Name}";
			}).ToPythonCode();
			Engine.Execute(importMethods, Scope);
		}

		private void ImportTypes(IEnumerable<ImportType> types)
		{
			foreach (ImportType import in types)
			{
				// Import all in temp object RSImport
				Engine.Execute($"import {import.Type.Namespace}.{import.Type.Name} as RSImport", Scope);
				// Create object that will contains every renamed (or not renamed) member
				Engine.Execute($"{import.Name} = RenSharpWrapper()", Scope);

				ImportMembers(import);

				Engine.Execute($"del RSImport", Scope);
			}
		}

		private void ImportMembers(ImportType import)
		{
			MemberInfo[] members = import.Type
				.GetMembers(BindingFlags.Public | BindingFlags.Static)
				.Where(x => !(x is MethodInfo) || !(x as MethodInfo).IsSpecialName)
				.ToArray();

			foreach(MemberInfo member in members)
			{
				var attr = member.GetCustomAttribute<PyImportAttribute>();

				if (attr != null)
					RSImport(import.Name, attr.Name, member.Name);
				else
					RSImport(import.Name, member.Name, member.Name);
			}
		}

		private void RSImport(string target, string name, string member)
		{
			if (target == "RSImport")
				throw new ArgumentException("Вы не можете использовать системное имя 'RSImport' как название вашего класса");
			// Not import if starts with underscore
			if (name.StartsWith("_"))
				return;
			Engine.Execute($"{target}.{name} = RSImport.{member}", Scope);
		}

		private void AddClrReferences()
		{
			// Get all types of imported methods and imported types
			// Find assemblies of it's types
			List<string> assemblies = PyImportAttribute.MethodImports
				.Select(m => m.MethodInfo.DeclaringType)
				.Union(PyImportAttribute.TypeImports.Select(x => x.Type))
				.Select(x => x.Assembly.FullName)
				.Distinct()
				.ToList();

			string loadAssemblies = string.Join("\n", assemblies
				.Select(assembly => $@"clr.AddReference(""{assembly}"")")
				.Distinct());

			Engine.Execute("import clr", Scope);
			Engine.Execute(loadAssemblies, Scope);
		}

		private void ImportSystemTypes()
		{
			string renSharpWrapper = @"
class RenSharpWrapper():
	def __setattr__(self, name, value):
		self.__dict__[name] = value
";
			Engine.Execute(renSharpWrapper, Scope);
		}

	}
}