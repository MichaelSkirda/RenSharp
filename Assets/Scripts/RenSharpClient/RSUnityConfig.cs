using RenSharp;
using RenSharp.Core;

public static class RSUnityConfig
{
	public static Configuration GetDefault()
	{
		Configuration config = DefaultConfiguration.GetDefaultConfig();

		config.SetCommand("show", (words, _) => CommandParsers.ParseShow(words));

		return config;
	}
}
