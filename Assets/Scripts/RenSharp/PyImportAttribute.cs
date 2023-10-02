﻿using RenSharp.Core;
using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace RenSharp
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class PyImportAttribute : Attribute
	{
		public static List<ImportMethod> MethodImports { get; private set; } = new List<ImportMethod>();
		public static List<ImportType> TypeImports { get; private set; } = new List<ImportType>();

		private string Name { get; set; }

		public PyImportAttribute(string name = "")
		{
			Name = name;
		}

		internal static void ReloadCallbacks()
		{
			MethodImports = new List<ImportMethod>();
			TypeImports = new List<ImportType>();

			List<Type> domainTypes = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.ToList();

			// Get ALL methods with attribute in ALL assemblies from Domain
			List<MethodInfo> importMethods = domainTypes
				.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public))
				.Where(x => x.IsDefined(typeof(PyImportAttribute)))
				.ToList();

			// Get ALL types with attribute in ALL assemblies from Domain
			List<Type> importTypes = domainTypes
				.Where(x => x.IsDefined(typeof(PyImportAttribute)))
				.ToList();


			foreach(MethodInfo method in importMethods)
			{
				PyImportAttribute attribute = method.GetCustomAttribute<PyImportAttribute>();
				var import = new ImportMethod(method, attribute.Name);
				MethodImports.Add(import);
			}

			foreach(Type type in importTypes)
			{
				PyImportAttribute attribute = type.GetCustomAttribute<PyImportAttribute>();
				var import = new ImportType(type, attribute.Name);
				TypeImports.Add(import);
			}

			AssertDuplicates(MethodImports);
		}

		private static void AssertDuplicates(List<ImportMethod> methods)
		{
			// MethodInfo names
			IEnumerable<string> noCustomName = methods
				.Where(x => string.IsNullOrWhiteSpace(x.Name))
				.Select(x => x.MethodInfo.Name);

			// User provided custom names
			IEnumerable<string> withCustomName = methods
				.Where(x => string.IsNullOrWhiteSpace(x.Name) == false)
				.Select(x => x.Name);

			AssertDuplicates(noCustomName);
			AssertDuplicates(withCustomName);
		}

		private static void AssertDuplicates(IEnumerable<string> values)
		{
			if (values.Count() != values.Distinct().Count())
				throw new ArgumentException("Methods or classes has duplicate name!");
		}
	}
}
