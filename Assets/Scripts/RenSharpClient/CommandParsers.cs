using Assets.Scripts.RenSharpClient;
using Assets.Scripts.RenSharpClient.Controllers;
using Assets.Scripts.RenSharpClient.Models.Commands;
using RenSharp;
using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

internal static class CommandParsers
{

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
		if (words.Count() < 1)
			throw new ArgumentException("Команда 'show' должна указывать какой спрайт показать.");

		string name = words[1];
		if(ReservedWords.Contains(name))
			throw new ArgumentException($"Слово '{words[1]}' зарезервированно. Вы не можете использовать его как имя.");

		string details = words.Skip(2).Until(ReservedWords).ToWord();
		IEnumerable<string> attributesWords = words.Skip(1).From(ReservedWords);
		Attributes attributes = AttributeParser.ParseAttributes(AttributesNames, attributesWords);

		return new Show(name, details, attributes, controller);
	}

	internal static Hide ParseHide(string[] words, ImageController controller)
	{
		if (words.Count() < 1)
			throw new ArgumentException("Команда 'hide' должна указывать какой спрайт спрятать.");

		if (words.Count() > 2)
			throw new ArgumentException($"Команда 'hide' может скрывать только 1 спрайт одноверменно. Лишние аргументы '{words.ToWord()}'.");

		string name = words[1];
		return new Hide(name, controller);
	}

	internal static Play ParsePlay(string[] words, SoundController controller)
	{
		if (words.Count() != 3)
			throw new ArgumentException($"Команда 'play' должна содержать 2 аргумента. Команда должна быть формата:'play sound/music [name]'. Текущее значение: '{words.ToWord()}'");

		bool isMusic;
		if (words[1] == "music")
			isMusic = true;
		else if (words[1] == "sound")
			isMusic = false;
		else
			throw new ArgumentException($"Команда 'play' может включать либо музыку (play music), либо звук (play sound). Текущее значение: '{words[1]}'");

		string name = words[2];
		if (string.IsNullOrEmpty(name))
			throw new ArgumentException("Команда 'play' должна указывать имя звука/музыки для запуска. Формат команды: 'play sound/music [name]'");

		return new Play(name, isMusic, controller);
	}

}
