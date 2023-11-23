using RenSharp.Core.Parse;
using RenSharp.Interfaces;
using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenSharp
{
    public class Configuration
	{
		private List<Type> SkipCommands = new List<Type>();
		private Dictionary<string, string> DefaultAttributes { get; set; } = new Dictionary<string, string>();
		private Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
		private List<string> Keywords { get; set; } = new List<string>();

		public T GetValueOrDefault<T>(string key)
		{
			try
			{
				// Won't catch key not found
				return (T)Values[key];
			}
			catch(InvalidCastException)
			{
				return default;
			}
		}
		public void SetValue(string key, object value) => Values[key] = value;

		public Dictionary<string, Func<string[], Configuration, Command>> CommandParsers { get; set; }
			= new Dictionary<string, Func<string[], Configuration, Command>>();

		public Dictionary<Type, Func<ParserContext, Command, List<Command>>> ComplexCommandParsers { get; set; }
			= new Dictionary<Type, Func<ParserContext, Command, List<Command>>>();

		public Dictionary<Type, List<Predicate<Command>>> IsComplexPredicates { get; set; }
			= new Dictionary<Type, List<Predicate<Command>>>();

		internal List<Type> AllowedToPushStack { get; set; } = new List<Type>();

		internal List<Type> MustPushStack { get; set; } = new List<Type>();

		public List<Command> ParseComplex(ParserContext ctx, Command command)
			=> ComplexCommandParsers[command.GetType()](ctx, command);
		public bool CanPush(Command command) => AllowedToPushStack.Contains(command.GetType());
		public void SetCommand(string command, Func<string[], Configuration, Command> Parser)
		{
			CommandParsers[command] = Parser;
			Keywords.Add(command);
		}
		public void DelCommand(string key) => CommandParsers.Remove(key);
		public void SetDefault(string key, string value) => DefaultAttributes[key] = value;
		
		public bool IsComplex(Command command)
			=> IsComplexPredicates.ContainsKey(command.GetType()) 
			&& IsComplexPredicates[command.GetType()].Any(x => x(command));

		public bool IsKeyword(string word)
			=> Keywords.Contains(word);

		public void SetComplexPredicate<T>(Predicate<Command> predicate) where T : Command
		{
			Type type = typeof(T);
			bool hasValue = IsComplexPredicates.ContainsKey(type);
			if (hasValue == false)
				IsComplexPredicates[type] = new List<Predicate<Command>>();

			IsComplexPredicates[typeof(T)].Add(predicate);
		}

		public void AddComplex(Type type, Func<ParserContext, Command, List<Command>> Parser)
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

		public bool IsNotSkip(Command command)
			=> !IsSkip(command);

		public string GetDefaultValue(string attributeName) => DefaultAttributes[attributeName];
		public string GetDefaultKeyValueString(string attributeName)
			=> $"{attributeName}={DefaultAttributes[attributeName]}";
		public Attributes GetDefaultAttrbutes()
			=> new Attributes(DefaultAttributes);

		public IWriter Writer { get; set; }
	}
}
