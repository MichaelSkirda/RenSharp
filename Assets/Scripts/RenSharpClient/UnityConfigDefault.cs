using Newtonsoft.Json;
using RenSharp;
using RenSharpClient.Controllers;
using RenSharpClient.Models.Commands;
using RenSharpClient.Models.Commands.Json;
using RenSharpClient.Models.Models.Commands;
using RenSharpClient.Parser.Complex;
using System.Linq;


public static class UnityConfigDefault
{
	public static Configuration GetDefault
		(ImageController imageController, SoundController soundController, MenuController menuController)
	{
		Configuration config = DefaultConfiguration.GetDefaultConfig();

		config.SetCommand("show", (words, _) => CommandParsers.ParseShow(words, imageController));
		config.SetCommand("hide", (words, _) => CommandParsers.ParseHide(words, imageController));
		config.SetCommand("image", (words, _) => CommandParsers.ParseImage(words, imageController));
		config.SetCommand("scene", (words, _) => CommandParsers.ParseScene(words, imageController));

		config.SetCommand("play", (words, _) => CommandParsers.ParsePlay(words, soundController));
		config.SetCommand("queue", (words, _) => CommandParsers.ParseQueue(words, soundController));
		config.SetCommand("stop", (words, _) => CommandParsers.ParseStop(words, soundController));

		config.SetCommand("menu", (_, _) => CommandParsers.ParseMenu(menuController));

		config.AddComplex(typeof(Menu), (ctx, rootCmd) => MenuComplexParser.Parse(ctx, rootCmd as Menu));
		config.AddComplex(typeof(ImageCommand), (ctx, rootCmd) => ImageComplexParser.Parse(ctx, rootCmd as ImageCommand));

		config.SetComplexPredicate<Menu>(x => true);
		config.SetComplexPredicate<ImageCommand>(x => true);

		config.SetDefault("at", "center"); // if not specified sprites appears at center
		config.SetDefault("fullscreen", "false");

		config.Skip<Show>();
		config.Skip<Scene>();
		config.Skip<Hide>();

		config.Skip<Play>();
		config.Skip<StopMusic>();

		config.Skip<ImageCommand>();
		config.Skip<MenuRollback>();

		config.Skip<SceneRollback>();

		config.MustPush<Menu>();

		SetJsonParsers(config, imageController, soundController, menuController);
		SetVariables(config);

		return config;
	}

	private static void SetJsonParsers
		(Configuration config, ImageController imageController, SoundController soundController, MenuController menuController)
	{
		config.DeserializeParsers.Add("show", (json, core) =>
		{
			var jsonParsed = JsonConvert.DeserializeObject<ShowJson>(json);
            var command = new Show(jsonParsed.Name, jsonParsed.Details, jsonParsed.Attributes, imageController);
            command.SetPosition(jsonParsed);
            return command;
		});

        config.DeserializeParsers.Add("scene", (json, core) =>
        {
            var jsonParsed = JsonConvert.DeserializeObject<ShowJson>(json);
            var command = new Scene(jsonParsed.Name, jsonParsed.Details, jsonParsed.Attributes, imageController);
            command.SetPosition(jsonParsed);
            return command;
        });

        config.DeserializeParsers.Add("hide", (json, core) =>
        {
			var jsonParsed = JsonConvert.DeserializeObject<HideJson>(json);
			var command = new Hide(jsonParsed.Name, jsonParsed.Attributes, imageController);
            command.SetPosition(jsonParsed);
            return command;
        });

        config.DeserializeParsers.Add("menu", (json, core) =>
        {
            var jsonParsed = JsonConvert.DeserializeObject<MenuJson>(json);
            var command = new Menu(menuController) { Buttons = jsonParsed.Buttons.ToList() };
            command.SetPosition(jsonParsed);
            return command;
        });


    }

	private static void SetVariables(Configuration config)
	{
        config.SetValue("screen_height", 1080);
        config.SetValue("screen_width", 1920);
        config.SetValue("gui_btn_gap", 50f);
        config.SetValue("gui_btn_first_gap", 25f);
        config.SetValue("rollback_cooldown", 300);
        config.SetValue("fast_forward_delay", 200); // Ctrl makes 5 commands per sec. (1000ms / 200ms)
    }

}
