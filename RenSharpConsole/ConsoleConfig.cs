using RenSharp;
using RenSharp.Interfaces;
using RenSharpConsole.Commands;
using System.Runtime.Serialization;

namespace RenSharpConsole
{
	internal static class ConsoleConfig
	{
		internal static Configuration GetDefaultConfig(IFormatter formatter, IWriter writer)
		{
			Configuration config = GetDefaultConfig();
			config.Writer = writer;

			config.SetCommand("color", (words, _)
				=> words.Count() == 2 ? new TextColor(words[1], formatter) : throw new ArgumentException());

			return config;
		}

		internal static Configuration GetDefaultConfig()
		{
			Configuration config = DefaultConfiguration.GetDefaultConfig();
			return config;
		}
	}
}
