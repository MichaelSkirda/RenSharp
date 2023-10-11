using Assets.Scripts.RenSharpClient.Controllers;
using RenSharp;

public static class RSUnityConfig
{
	public static Configuration GetDefault(ImageController imageController, SoundController soundController)
	{
		Configuration config = DefaultConfiguration.GetDefaultConfig();

		config.SetCommand("show", (words, _) => CommandParsers.ParseShow(words, imageController));
		config.SetCommand("hide", (words, _) => CommandParsers.ParseHide(words, imageController));
		config.SetCommand("play", (words, _) => CommandParsers.ParsePlay(words, soundController));

		return config;
	}
}
