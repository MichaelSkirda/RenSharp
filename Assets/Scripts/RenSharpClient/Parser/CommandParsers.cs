using RenSharp;
using RenSharp.Core.Parse;
using RenSharp.Models;
using RenSharp.Models.Parse;
using RenSharpClient.Controllers;
using RenSharpClient.Models.Commands;
using RenSharpClient.Models.Models.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using RenSharp.Core.Exceptions;

internal static class CommandParsers
{
	private static int AnonymousId { get; set; }
	private static string GetAnonymousId()
	{
		AnonymousId++;
		return $"_rs_anonymous_resource_{AnonymousId}";
	}

	private static List<string> ReservedWords { get; set; } = new List<string>()
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
		"queue"
	};

	private static List<string> AttributesNames { get; set; } = new List<string>()
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

	internal static ImageCommand ParseImage(string[] words, ImageController controller)
	{
		if (words.Count() < 2)
			throw new ArgumentException("������� 'image' ������ ��������� ��� ������� �� ImageController.");

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
			throw new ArgumentException("������� 'hide' ������ ��������� ����� ������ ��������.");

		string[] allowedAttributes = { "with" };
		Attributes attributes = AttributeParser.ParseAttributes(allowedAttributes, words.Skip(2));
		string name = words[1];
		return new Hide(name, attributes, controller);
	}

	internal static StopMusic ParseStop(string[] words, SoundController controller)
	{
		if (words.Count() < 2)
			throw new ArgumentException("������� 'stop' ������ ��������� ������� 1 ��������. ������ 'stop music/sound' [�� ����������� fadeout X.X]");

		string channel = words[1];

		string[] allowedAttributes = { "fadeout" };
		Attributes attributes = AttributeParser.ParseAttributes(allowedAttributes, words.Skip(2));
		return new StopMusic(channel, attributes, controller);
	}

	internal static Play ParsePlay(string[] words, SoundController controller)
	{
		if (words.Count() < 3)
			throw new WrongArgumentNumberException($"������� 'play' ������ ��������� ������� 2 ���������. ������� ������ ���� �������:'play [channel] [name] [effect (�����������)]'. ������� ��������: '{words.ToWord()}'");

		string channel = words[1];
		if (RegexMethods.IsValidCharacterName(channel) == false)
			throw new ArgumentException("�������� ����� �� ������ ��������� ����������� ������� (����� ������� '_')." +
				"�� ������ ������������ ���� �� ����������� ������� 'music', 'sound', 'voice'.");

		ArrayParsed parsed;

		try
		{
			parsed = ArrayParser.ParseArrayStrict(string.Join(" ", words.Skip(2)));
        }
		catch(ArgumentException)
		{
			StringFirstQuotes valueInQuotes = RegexMethods.BetweenQuotesFirst(words.Skip(2));
			parsed = new ArrayParsed()
			{
				Values = new List<string>() { valueInQuotes.Between },
				After = valueInQuotes.After
			};
		}
		catch
		{
			throw;
		}

        IEnumerable<string> names = parsed.Values;

		foreach(string name in names)
		{
            string path = Path.ChangeExtension(name, extension: null);

            var audioClips = Resources.LoadAll<AudioClip>(path).ToList();

            try
            {
                if (audioClips.Count > 1)
                    throw new ArgumentException($"������� ����� ������ ��������� ����� '{name}'. ����� ���� �� ���� ������ Resources �� �������� ���������� ������.");
                else if (audioClips.Count <= 0)
                    throw new ArgumentException($"�������� ���� '{name}' �� ������. ���� ���� ��������� ������������ ����� Resources. ����� ���� �� ���� ������ Resources.");
            }
            catch
            {
                audioClips.ForEach(x => Resources.UnloadAsset(x));
                throw;
            }
            controller.AddAudio(path, audioClips.First());
        }

        IEnumerable<string> attributesWords = parsed.After.Split(' ');

		var allowedAttributes = new string[] { "fadein", "fadeout", "volume" };
		Attributes attributes = AttributeParser.ParseAttributes(allowedAttributes, attributesWords);

		return new Play(names, channel, attributes, controller);
	}

	internal static QueueCommand ParseQueue(string[] words, SoundController controller)
	{
		try
		{
            Play play = ParsePlay(words, controller);
            var queue = new QueueCommand(play.ClipNames, play.Channel, play.Attributes, play.Controller);
            return queue;
        }
		catch(WrongArgumentNumberException ex)
		{
			throw new WrongArgumentNumberException("������� 'queue' ������ ����� ������� 2 ���������." +
				" ������� ������ ���� �������: 'queue [channel] [name] [effect (�����������)]'." +
				$"������� ��������: '{words.ToWord()}'", ex);
		}
		catch
		{
			throw;
		}
        
	}

	internal static Menu ParseMenu(MenuController controller) => new Menu(controller);
}
