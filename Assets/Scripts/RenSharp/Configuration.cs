using RenSharp.Core;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharp.Models.Commands;
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


		public Dictionary<string, Func<string[], Configuration, Command>> CommandParsers { get; set; }
			= new Dictionary<string, Func<string[], Configuration, Command>>();

		public Dictionary<Type, Func<ReaderContext, Command, List<Command>>> ComplexCommandParsers { get; set; }
			= new Dictionary<Type, Func<ReaderContext, Command, List<Command>>>();

		public Dictionary<Type, List<Predicate<Command>>> IsComplexPredicates { get; set; }
			= new Dictionary<Type, List<Predicate<Command>>>();

		internal List<Type> AllowedToPushStack { get; set; } = new List<Type>();

		internal List<Type> MustPushStack { get; set; } = new List<Type>();

		public List<Command> ParseComplex(ReaderContext ctx, Command command)
			=> ComplexCommandParsers[command.GetType()](ctx, command);
		public bool CanPush(Command command) => AllowedToPushStack.Contains(command.GetType());
		public void SetCommand(string command, Func<string[], Configuration, Command> Parser)
			=> CommandParsers[command] = Parser;
		public void SetDefault(string key, string value) => DefaultAttributes[key] = value;
		public string GetDefaultValue(string attributeName) => DefaultAttributes[attributeName];
		public string GetDefaultKeyValueString(string attributeName)
			=> $"{attributeName}={DefaultAttributes[attributeName]}";
		public bool IsComplex(Command command)
			=> IsComplexPredicates.ContainsKey(command.GetType()) 
			&& IsComplexPredicates[command.GetType()].Any(x => x(command));
		public void SetComplexPredicate<T, K>(Predicate<K> predicate) where K : Command
		{

		}

		public void AddComplex(Type type, Func<ReaderContext, Command, List<Command>> Parser)
			=> ComplexCommandParsers[type] = Parser;
		public bool IsMustPush(Command command) => MustPushStack.Contains(command.GetType());
		public void MustPush<T>()
		{
			// If not implements IPushable
			if (typeof(IPushable).IsAssignableFrom(typeof(T)) == false)
				throw new ArgumentException($"Command of type '{typeof(T)}' not implements IPushable, but configured as 'Must to push'.");

			// If command must push it automaticly can push
			AllowToPushStack<T>();
			MustPushStack.Add(typeof(T));
		}

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

		public bool IsSkip(Command command)
		{
			Type type = command.GetType();
			return SkipCommands.Contains(type);
		}

		public IWriter Writer { get; set; }
	}
}
