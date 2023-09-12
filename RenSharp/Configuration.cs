using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp
{
	public class Configuration
	{
		private List<Type> SkipCommands = new List<Type>();

		public void Skip<T>()
		{
			Type type = typeof(T);
			SkipCommands.Add(type);
		}

		public bool IsSkip<T>()
		{
			Type type = typeof(T);
			return SkipCommands.Contains(type);
		}

		public bool IsSkip<T>(T command)
		{
			Type type = command.GetType();
			return SkipCommands.Contains(type);
		}
	}
}
