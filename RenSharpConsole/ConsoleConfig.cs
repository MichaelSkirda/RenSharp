using RenSharp;
using RenSharp.Interfaces;
using RenSharpConsole.Commands;

namespace RenSharpConsole
{
	internal static class ConsoleConfig
	{
		internal static Configuration GetDefaultConfig(IFormatter formatter, IWriter writer)
		{
			Configuration config = DefaultConfiguration.GetDefaultConfig();
			config.Writer = writer;

			config.Skip<TextColor>();

			config.SetCommand("color", (words, _)
				=> words.Count() == 2 ? new TextColor(words[1], formatter) : throw new ArgumentException());

			return config;
		}
	}
}
