using Assets.Scripts.RenSharpClient;
using RenSharp;
using RenSharp.Core.Parse;
using RenSharp.Models;
using RenSharp.Models.Parse;
using RenSharpClient.Controllers;
using RenSharpClient.Models.Commands;
using RenSharpClient.Models.Models.Commands;
using Scripts.RenSharpClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

internal static class CommandParsers
{
	private static int AnonymousId { get; set; }
	private static string GetAnonymousId()
	{
		AnonymousId++;
		return $"_rs_anonymous_resource_{AnonymousId}";
	}

	private static List<string> ReservedWords = new List<string>()
	{
		// Control
		"label",
		"jump",
		"call",

		// Images
		"show",
		"hide",
		"scene",

		// Sound
		"play",
		"stop",
	};

	private static List<string> AttributesNames = new List<string>()
	{
		// Attributes
		"at",
		"with",
		"fullscreen"
	};

	static CommandParsers()
	{
		ReservedWords.AddRange(AttributesNames);
	}

	internal static Show ParseShow(string[] words, ImageController controller)
	{
		if (words.Count() < 2)
			throw new ArgumentException("Команда 'show' должна указывать какой спрайт показать.");

		string name = words[1];
		if(ReservedWords.Contains(name))
			throw new ArgumentException($"Слово '{words[1]}' зарезервированно. Вы не можете использовать его как имя.");

		string details = words.Skip(2).Until(ReservedWords).ToWord();
		IEnumerable<string> attributesWords = words.Skip(1).From(ReservedWords);
		Attributes attributes = AttributeParser.ParseAttributes(AttributesNames, attributesWords);

		return new Show(name, details, attributes, controller);
	}

	internal static Scene ParseScene(string[] words, ImageController controller)
	{
		Show show = ParseShow(words, controller);
		return new Scene(show.Name, show.Details, show.Attributes, show.Controller);
	}

	internal static ImageCommand ParseImage(string[] words, ImageController controller)
	{
		if (words.Count() < 2)
			throw new ArgumentException("Команда 'image' должна указывать имя спрайта из ImageController.");

		string name = words[1];
		string details = words.Skip(2).ToWord();
		if (string.IsNullOrWhiteSpace(details))
			details = string.Empty;

		var attributes = new Attributes();

		var image = new ImageCommand(name, details, attributes, controller);
		return image;
	}

	internal static Hide ParseHide(string[] words, ImageController controller)
	{
		if (words.Count() < 1)
			throw new ArgumentException("Команда 'hide' должна указывать какой спрайт спрятать.");

		string[] allowedAttributes = { "with" };
		Attributes attributes = AttributeParser.ParseAttributes(allowedAttributes, words.Skip(2));
		string name = words[1];
		return new Hide(name, attributes, controller);
	}

	internal static StopMusic ParseStop(string[] words, SoundController controller)
	{
		if (words.Count() < 2)
			throw new ArgumentException("Команда 'stop' должна содержать минимум 1 аргумент. Фомарт 'stop music/sound' [не обязательно fadeout X.X]");

		string channel = words[1];

		string[] allowedAttributes = { "fadeout" };
		Attributes attributes = AttributeParser.ParseAttributes(allowedAttributes, words.Skip(2));
		return new StopMusic(channel, attributes, controller);
	}

	internal static Play ParsePlay(string[] words, SoundController controller)
	{
		if (words.Count() < 3)
			throw new ArgumentException($"Команда 'play' должна содержать минимум 2 аргумента. Команда должна быть формата:'play [channel] [name] [effect (опционально)]'. Текущее значение: '{words.ToWord()}'");

		string channel = words[1];

		StringFirstQuotes quotedValue = RegexMethods.BetweenQuotesFirst(words.Skip(2)); // Skip keyword play and channel
		string name = quotedValue.Between;
		if (quotedValue == null || string.IsNullOrEmpty(name))
			throw new ArgumentException(RSExceptionMessages.NoAudioPathSpecified);

		string path = quotedValue.Between;
		path = Path.ChangeExtension(path, extension: null);

		var audioClips = Resources.LoadAll<AudioClip>(path).ToList();

		try
		{
			if (audioClips.Count > 1)
				throw new ArgumentException($"Найдена более одного звукового файла '{path}'. Поиск идет по всем папкам Resources не учитывая расширения файлов.");
			else if (audioClips.Count <= 0)
				throw new ArgumentException($"Звуковой файл '{path}' не найден. Путь надо указывать относительно папки Resources. Поиск идет по всем папкам Resources.");
		}
		catch
		{
			audioClips.ForEach(x => Resources.UnloadAsset(x));
			throw;
		}

		controller.AddAudio(name, audioClips.First());

		IEnumerable<string> attributesWords = quotedValue.After.Split(' ');

		var allowedAttributes = new string[] { "fadein", "fadeout" };
		Attributes attributes = AttributeParser.ParseAttributes(allowedAttributes, attributesWords);

		return new Play(new List<string> { name }, channel, attributes, controller);
	}

	internal static Menu ParseMenu(MenuController controller) => new Menu(controller);
}
