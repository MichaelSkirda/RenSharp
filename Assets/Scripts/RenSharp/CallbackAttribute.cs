using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace RenSharp
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CallbackAttribute : Attribute
	{
		private static Dictionary<string, (object, MethodInfo)> Callbacks { get; set; }
			= new Dictionary<string, (object, MethodInfo)>();
		public static List<MethodInfo> MethodsList { get; set; }

		public CallbackAttribute(string name = "", [CallerMemberName] string memberName = default!)
		{
			if (MethodsList == null)
			{
				MethodsList = Assembly.GetAssembly(typeof(RenSharpCore))
				  .GetTypes()
				  .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
				  .ToList();
			}

			if (name == "")
				name = memberName;
			MethodInfo method = MethodsList.FirstOrDefault(m => m.Name == memberName);
			if (method == null || method.DeclaringType == null)
				return;

			Callbacks.Add(name, (null, method));
		}

		public static void RegisterMethod(string name, MethodInfo method, object obj = null)
			=> Callbacks[name] = (obj, method);

		public static void CallMethod(string name, RenSharpCore renSharp, string[] args)
		{
			if (MethodsList == null || MethodsList.Count <= 0)
			{
				_ = Assembly.GetAssembly(typeof(RenSharpCore))
					  .GetTypes()
					  .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					  .Where(m => m.GetCustomAttributes(typeof(CallbackAttribute), false).Length > 0)
					  .ToList();
			}


			(object, MethodInfo) objMethod;

			bool exist = Callbacks.TryGetValue(name, out objMethod);

			if (!exist)
				throw new ArgumentException($"Function '{name}' not exist.");

			// The class instance that contains method
			object obj = objMethod.Item1;
			MethodInfo method = objMethod.Item2;

			// Todo calculate args
			try
			{
				method.Invoke(obj, new object[] { renSharp, args });
			}
			catch
			{
				try
                {
					method.Invoke(obj, new object[] { args });
				}
				catch
                {
					method.Invoke(obj, new object[] {  });
				}
			}

		}

	}
}
