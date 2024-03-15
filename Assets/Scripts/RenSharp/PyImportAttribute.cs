using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RenSharp
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class PyImportAttribute : Attribute
	{
		public static List<ImportMethod> MethodImports { get; private set; } = new List<ImportMethod>();
		public static List<ImportType> StaticTypeImports { get; private set; } = new List<ImportType>();
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
			StaticTypeImports = new List<ImportType>();

			var assemblies = AppDomain.CurrentDomain
				.GetAssemblies()
				.Distinct()
				.ToList();

			List<Type> domainTypes = new List<Type>();

			foreach(Assembly assembly in assemblies)
			{
				try
				{
					var types = assembly.GetTypes();
					domainTypes.AddRange(types);
				}
				catch { }
			}

			domainTypes = domainTypes.Distinct().ToList();

			// Get ALL types with attribute in ALL assemblies from Domain
			List<Type> attributedTypes = domainTypes
				.Where(x => x.IsDefined(attr))
				.ToList();

			// Get ALL methods with attribute in ALL assemblies from Domain
			// Except methods that class contains PyImportAttribute
			List<MethodInfo> importMethods = domainTypes
				.Except(attributedTypes)
				.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public))
				.Where(x => x.IsDefined(attr))
				.ToList();

			// Get ALL static types with attribute
			// Only static types can be both abstract and sealed
			List<Type> importStaticTypes = attributedTypes
				.Where(x => x.IsAbstract && x.IsSealed)
				.ToList();

			List<Type> importTypes = attributedTypes
				.Where(x => !(x.IsAbstract && x.IsSealed))
				.ToList();



			foreach (MethodInfo method in importMethods)
			{
				PyImportAttribute attribute = GetPyImportAttribute(method);
				var import = new ImportMethod(method, attribute.Name);
				MethodImports.Add(import);
			}

			foreach(Type type in importStaticTypes)
			{
				PyImportAttribute attribute = GetPyImportAttribute(type);
				var import = new ImportType(type, attribute.Name);
				StaticTypeImports.Add(import);
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
