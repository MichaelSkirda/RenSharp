using Assets.Scripts.RenSharpClient.Commands;
using Assets.Scripts.RenSharpClient.Controllers;
using Assets.Scripts.RenSharpClient.Models.Commands;
using RenSharp;

public static class RSUnityConfig
{
	public static Configuration GetDefault(ImageController imageController, SoundController soundController)
	{
		Configuration config = DefaultConfiguration.GetDefaultConfig();

		config.SetCommand("show", (words, _) => CommandParsers.ParseShow(words, imageController));
		config.SetCommand("hide", (words, _) => CommandParsers.ParseHide(words, imageController));
		config.SetCommand("play", (words, _) => CommandParsers.ParsePlay(words, soundController));

		config.SetDefault("at", "center"); // if not specified sprites appears at center

		config.Skip<Show>();
		config.Skip<Hide>();
		config.Skip<Play>();

		return config;
	}
}
