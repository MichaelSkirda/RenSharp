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
	[AttributeUsage(AttributeTargets.Method)]
	public class CallbackAttribute : Attribute
	{
		public static List<RenSharpMethod> Callbacks { get; set; } = new List<RenSharpMethod>();
		private static List<MethodInfo> StaticMethods { get; set; } = new List<MethodInfo>();

		public CallbackAttribute(string ns = "", [CallerMemberName] string memberName = default!)
		{
			List<MethodInfo> methods = StaticMethods
				.Where(x => x.Name == memberName)
				.ToList();

			if (methods.Count() > 1)
				throw new Exception($"There are more than one method with name {memberName}");

			MethodInfo method = methods.First();

			if (method == null || method.DeclaringType == null)
				throw new Exception($"Can not parse method with name {memberName}");

			Callbacks.Add(new RenSharpMethod(method, ns));
		}

		internal static void Init()
		{
			Callbacks = new List<RenSharpMethod>();
			if (StaticMethods == null || StaticMethods.Count <= 0)
			{
				// Load ALL methods with attribute in ALL assembilies from Domain
				StaticMethods = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(x => x.GetTypes())
					.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public))
					.Where(x => x.IsDefined(typeof(CallbackAttribute)))
					.ToList();

				// Call contructors
				StaticMethods
					.ForEach(x => x.GetCustomAttributes(typeof(CallbackAttribute), false));
			}
			StaticMethods = null;
		}

		public static void RegisterMethod(string ns, MethodInfo method)
			=> Callbacks.Add(new RenSharpMethod(method, ns));
	}
}
