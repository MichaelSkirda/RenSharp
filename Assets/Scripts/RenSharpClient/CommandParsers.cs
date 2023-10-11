using Assets.Scripts.RenSharpClient.Commands;
using Assets.Scripts.RenSharpClient.Controllers;
using Assets.Scripts.RenSharpClient.Models.Commands;
using RenSharp;
using System;
using System.Collections.Generic;
using System.Linq;

internal static class CommandParsers
{

	internal static Show ParseShow(string[] words, ImageController controller)
	{
		if (words.Count() < 1)
			throw new ArgumentException("Команда 'show' должна указывать какой спрайт показать.");

		string name = words[1];
		string details = words.Skip(2).ToWord();
		var attributes = new List<string>();
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
