using Assets.Scripts.RenSharpClient.Controllers;
using Assets.Scripts.RenSharpClient.Models.Commands;
using Assets.Scripts.RenSharpClient.Parser.Complex;
using RenSharp;

public static class RSUnityConfig
{
	public static Configuration GetDefault(ImageController imageController, SoundController soundController)
	{
		Configuration config = DefaultConfiguration.GetDefaultConfig();

		config.SetCommand("show", (words, _) => CommandParsers.ParseShow(words, imageController));
		config.SetCommand("hide", (words, _) => CommandParsers.ParseHide(words, imageController));
		config.SetCommand("image", (words, _) => CommandParsers.ParseImage(words, imageController));
		config.SetCommand("play", (words, _) => CommandParsers.ParsePlay(words, soundController));

		config.AddComplex(typeof(Image), (ctx, rootCmd) => ImageComplexParser.Parse(ctx, rootCmd as Image));

		config.SetComplexPredicate<Image>(x => true);

		config.SetDefault("at", "center"); // if not specified sprites appears at center
		config.SetDefault("fullscreen", "false");

		config.Skip<Show>();
		config.Skip<Hide>();
		config.Skip<Play>();

		config.SetValue("screen_height", 1080);
		config.SetValue("screen_width", 1920);

		return config;
	}
}
