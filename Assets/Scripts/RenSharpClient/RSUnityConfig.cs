using RenSharp;

public static class RSUnityConfig
{
	public static Configuration GetDefault(ImageController imageController)
	{
		Configuration config = DefaultConfiguration.GetDefaultConfig();

		config.SetCommand("show", (words, _) => CommandParsers.ParseShow(words, imageController));

		return config;
	}
}
