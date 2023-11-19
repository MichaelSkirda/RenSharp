using Assets.Scripts.RenSharpClient.Controllers;
using Assets.Scripts.RenSharpClient.Models.Commands;
using Assets.Scripts.RenSharpClient.Parser.Complex;
using RenSharp;

public static class UnityConfigDefault
{
	public static Configuration GetDefault
		(ImageController imageController, SoundController soundController, MenuController menuController)
	{
		Configuration config = DefaultConfiguration.GetDefaultConfig();

		config.SetCommand("show", (words, _) => CommandParsers.ParseShow(words, imageController));
		config.SetCommand("hide", (words, _) => CommandParsers.ParseHide(words, imageController));
		config.SetCommand("image", (words, _) => CommandParsers.ParseImage(words, imageController));
		config.SetCommand("play", (words, _) => CommandParsers.ParsePlay(words, soundController));
		config.SetCommand("menu", (_, _) => CommandParsers.ParseMenu(menuController));

		config.AddComplex(typeof(Menu), (ctx, rootCmd) => MenuComplexParser.Parse(ctx, rootCmd as Menu));
		config.AddComplex(typeof(Image), (ctx, rootCmd) => ImageComplexParser.Parse(ctx, rootCmd as Image));

		config.SetComplexPredicate<Menu>(x => true);
		config.SetComplexPredicate<Image>(x => true);

		config.SetDefault("at", "center"); // if not specified sprites appears at center
		config.SetDefault("fullscreen", "false");

		config.Skip<Show>();
		config.Skip<Hide>();
		config.Skip<Play>();
		config.Skip<Image>();

		config.MustPush<Menu>();

		config.SetValue("screen_height", 1080);
		config.SetValue("screen_width", 1920);
		config.SetValue("gui_btn_gap", 50f);
		config.SetValue("gui_btn_first_gap", 25f);

		return config;
	}
}
