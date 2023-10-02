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
	public class CallbackAttribute : Attribute
	{
		public static List<RenSharpMethod> Callbacks { get; private set; } = new List<RenSharpMethod>();

		private static List<MethodInfo> CallbackMethods { get; set; } = new List<MethodInfo>();
		private static List<Type> CallbackTypes { get; set; } = new List<Type>();

		public CallbackAttribute(string ns = "", [CallerMemberName] string memberName = default!)
		{
			List<MethodInfo> methods = CallbackMethods
				.Where(x => x.Name == memberName)
				.ToList();

			if (methods.Count() > 1)
				throw new Exception($"There are more than one method with name {memberName}");

			MethodInfo method = methods.First();

			if (method == null || method.DeclaringType == null)
				throw new Exception($"Can not parse method with name {memberName}");

			Callbacks.Add(new RenSharpMethod(method, ns));
		}

		internal static void ReloadCallbacks()
		{
			Callbacks = new List<RenSharpMethod>();

			List<Type> domainTypes = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.ToList();

			// Load ALL methods with attribute in ALL assembilies from Domain
			CallbackMethods = domainTypes
				.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public))
				.Where(x => x.IsDefined(typeof(CallbackAttribute))
					&& x.DeclaringType.IsDefined(typeof(CallbackAttribute)) == false)
				.ToList();

			CallbackTypes = domainTypes
				.Where(x => x.IsDefined(typeof(CallbackAttribute)))
				.ToList();

			// Call contructors
			CallbackMethods
				.ForEach(x => x.GetCustomAttributes(typeof(CallbackAttribute), false));
		}

		public static void RegisterMethod(string ns, MethodInfo method)
			=> Callbacks.Add(new RenSharpMethod(method, ns));
	}
}
