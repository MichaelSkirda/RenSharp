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
		public static Dictionary<string, (object, MethodInfo)> Callbacks { get; set; }
			= new Dictionary<string, (object, MethodInfo)>();
		public static List<MethodInfo> MethodsList { get; set; } = new List<MethodInfo>();

		public CallbackAttribute([CallerMemberName] string memberName = default!)
		{
			List<MethodInfo> methods = MethodsList
				.Where(x => x.Name == memberName && x.IsDefined(typeof(CallbackAttribute)))
				.ToList();

			if (methods.Count() > 1)
				throw new Exception($"There are more than one method with name {memberName}");

			MethodInfo method = methods.First();

			if (method == null || method.DeclaringType == null)
				throw new Exception($"Can not parse method with name {memberName}");

			Callbacks.Add(memberName, (null, method));
		}

		public static void Init()
		{
			if (MethodsList == null || MethodsList.Count <= 0)
			{
				// Load ALL methods with attribute in ALL assembilies from Domain
				MethodsList = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(x => x.GetTypes())
					.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public))
					.Where(x => x.IsDefined(typeof(CallbackAttribute)))
					.ToList();

				// Call contructors
				MethodsList
					.ForEach(x => x.GetCustomAttributes(typeof(CallbackAttribute), false));
			}
		}

		public static void RegisterMethod(string name, MethodInfo method, object obj = null)
			=> Callbacks[name] = (obj, method);

		public static void CallMethod(string name, RenSharpCore renSharp, string[] args)
		{
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
