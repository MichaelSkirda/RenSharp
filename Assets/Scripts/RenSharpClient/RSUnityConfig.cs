using RenSharp;
using RenSharp.Core;

public static class RSUnityConfig
{
	public static Configuration GetConfig()
	{
		Configuration config = DefaultConfiguration.GetDefaultConfig();

		config.SetCommand("hard", (words, _) => CommandParser.ParseReturn(words));

		return config;
	}
}
