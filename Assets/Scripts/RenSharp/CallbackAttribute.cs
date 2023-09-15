using RenSharp.Core;
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
		private static Dictionary<string, MethodInfo> Callbacks { get; set; } = new Dictionary<string, MethodInfo>();
		public static List<MethodInfo> MethodsList { get; set; }

		public CallbackAttribute(string name = "", [CallerMemberName] string memberName = default!)
		{
			if (MethodsList == null)
			{
				MethodsList = Assembly.GetEntryAssembly()
				  .GetTypes()
				  .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
				  .ToList();
			}

			if (name == "")
				name = memberName;
			MethodInfo method = MethodsList.FirstOrDefault(m => m.Name == memberName);
			if (method == null || method.DeclaringType == null)
				return;

			Callbacks.Add(name, method);
		}

		public static void CallMethod(string name, RenSharpCore renSharp, RenSharpContext context)
		{
			if (MethodsList == null || MethodsList.Count <= 0)
			{
				_ = Assembly.GetEntryAssembly()
					  .GetTypes()
					  .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					  .Where(m => m.GetCustomAttributes(typeof(CallbackAttribute), false).Length > 0)
					  .ToList();
			}


			bool exist = Callbacks.TryGetValue(name, out MethodInfo method);
			if(!exist)
				throw new ArgumentException($"Function '{name}' not exist.");

			method.Invoke(null, new object[] { renSharp, context });
		}

	}
}
