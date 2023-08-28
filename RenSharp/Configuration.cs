using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp
{
	internal class Configuration
	{
		private List<Type> SkipCommands = new List<Type>();

		public void Skip<T>() where T : Command
		{
			Type type = typeof(T);
			SkipCommands.Add(type);
		}

		public bool IsSkip<T>() where T : Command
		{
			Type type = typeof(T);
			return SkipCommands.Contains(type);
		}

		public bool IsSkip<T>(T command) where T : Command
		{
			return IsSkip<T>();
		}
	}
}
