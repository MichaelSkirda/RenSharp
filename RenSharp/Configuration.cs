using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenSharp
{
	public class Configuration
	{
		private List<Type> SkipCommands = new List<Type>();
		private Dictionary<string, string> DefaultAttributes = new Dictionary<string, string>();
		public Dictionary<string, Func<string[], Configuration, Command>> Commands { get; set; }
			= new Dictionary<string, Func<string[], Configuration, Command>>();

		internal List<Type> AllowedToPushStack = new List<Type>();

		public bool CanPush(Command command) => AllowedToPushStack.Contains(command.GetType());
		public void AddCommand(string command, Func<string[], Configuration, Command> Parser) => Commands[command] = Parser;
		public void SetDefault(string key, string value) => DefaultAttributes[key] = value;
		public string GetDefaultValue(string attributeName) => DefaultAttributes[attributeName];
		public string GetDefault(string attributeName) => $"{attributeName}={DefaultAttributes[attributeName]}";

		public void AllowToPushStack<T>()
		{
			Type type = typeof(T);
			AllowedToPushStack.Add(type);
		}

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
