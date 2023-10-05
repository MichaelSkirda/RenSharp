using RenSharp.Core;
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


		internal string Name { get; set; }

		public PyImportAttribute(string name = "")
		{
			Name = name;
		}

		internal static void ReloadCallbacks()
		{
			Type attr = typeof(PyImportAttribute);
			MethodImports = new List<ImportMethod>();
			TypeImports = new List<ImportType>();

			List<Type> domainTypes = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.ToList();

			// Get ALL methods with attribute in ALL assemblies from Domain
			// Except methods that class contains PyImportAttribute
			List<MethodInfo> importMethods = domainTypes
				.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public))
				.Where(x => x.IsDefined(attr) && x.DeclaringType.IsDefined(attr) == false)
				.ToList();

			// Get ALL types with attribute in ALL assemblies from Domain
			List<Type> importTypes = domainTypes
				.Where(x => x.IsDefined(attr))
				.ToList();


			foreach(MethodInfo method in importMethods)
			{
				PyImportAttribute attribute = GetPyImportAttribute(method);
				var import = new ImportMethod(method, attribute.Name);
				MethodImports.Add(import);
			}

			foreach(Type type in importTypes)
			{
				PyImportAttribute attribute = GetPyImportAttribute(type);
				var import = new ImportType(type, attribute.Name);
				TypeImports.Add(import);
			}

			AssertDuplicates(MethodImports);
			// TODO types assert
		}

		private static PyImportAttribute GetPyImportAttribute(MemberInfo member)
		{
			PyImportAttribute attribute = member.GetCustomAttribute<PyImportAttribute>();
			if(string.IsNullOrWhiteSpace(attribute.Name))
				attribute.Name = member.Name;
			return attribute;
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
