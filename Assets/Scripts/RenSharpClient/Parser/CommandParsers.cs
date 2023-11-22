using Assets.Scripts.RenSharpClient;
using RenSharp;
using RenSharp.Core.Parse;
using RenSharp.Models;
using RenSharp.Models.Parse;
using RenSharpClient.Controllers;
using RenSharpClient.Models.Commands;
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
			throw new ArgumentException("������� 'show' ������ ��������� ����� ������ ��������.");

		string name = words[1];
		if(ReservedWords.Contains(name))
			throw new ArgumentException($"����� '{words[1]}' ����������������. �� �� ������ ������������ ��� ��� ���.");

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

	internal static Image ParseImage(string[] words, ImageController controller)
	{
		if (words.Count() < 2)
			throw new ArgumentException("������� 'image' ������ ��������� ��� ������� �� ImageController.");

		string name = words[1];
		string details = words.Skip(2).ToWord();
		if (string.IsNullOrWhiteSpace(details))
			details = string.Empty;

		var attributes = new Attributes();

		var image = new Image(name, details, attributes, controller);
		return image;
	}

	internal static Hide ParseHide(string[] words, ImageController controller)
	{
		if (words.Count() < 1)
			throw new ArgumentException("������� 'hide' ������ ��������� ����� ������ ��������.");

		if (words.Count() > 2)
			throw new ArgumentException($"������� 'hide' ����� �������� ������ 1 ������ ������������. ������ ��������� '{words.ToWord()}'.");

		string name = words[1];
		return new Hide(name, controller);
	}

	internal static Play ParsePlay(string[] words, SoundController controller)
	{
		if (words.Count() != 3)
			throw new ArgumentException($"������� 'play' ������ ��������� 2 ���������. ������� ������ ���� �������:'play sound/music [name]'. ������� ��������: '{words.ToWord()}'");

		bool isMusic;
		if (words[1] == "music")
			isMusic = true;
		else if (words[1] == "sound")
			isMusic = false;
		else
			throw new ArgumentException($"������� 'play' ����� �������� ���� ������ (play music), ���� ���� (play sound). ������� ��������: '{words[1]}'");

		string name = words[2];
		if (string.IsNullOrEmpty(name))
			throw new ArgumentException("������� 'play' ������ ��������� ��� �����/������ ��� �������. ������ �������: 'play sound/music [name]'");

		if(name.StartsWith("\""))
		{
			StringFirstQuotes quotedValue = CommandParser.BetweenQuotesFirst(words);
			string path = quotedValue.Between;
			path = Path.ChangeExtension(path, extension: null);

			var data = Resources.LoadAll<AudioClip>(path).ToList();

			try
			{
				if (data.Count > 1)
					throw new ArgumentException($"������� ����� ������ ��������� ����� '{path}'. ����� ���� �� ���� ������ Resources �� �������� ���������� ������.");
				else if (data.Count <= 0)
					throw new ArgumentException($"�������� ���� '{path}' �� ������. ���� ���� ��������� ������������ ����� Resources. ����� ���� �� ���� ������ Resources.");
			}
			catch
			{
				data.ForEach(x => Resources.UnloadAsset(x));
				throw;
			}

			name = GetAnonymousId();
			controller.AddAudio(name, data.First());
		}

		return new Play(name, isMusic, controller);
	}

	internal static Menu ParseMenu(MenuController controller) => new Menu(controller);
}
