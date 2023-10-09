using Assets.Scripts.RenSharpClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RenSharp;

namespace Assets.Scripts.RenSharpClient
{
	internal class DataStorage : MonoBehaviour
	{
		[SerializeField]
		private SpriteItem[] _spriteItems;
		internal Dictionary<string, List<CharacterSprite>> CharactersSprites { get; set; }

		private void SetCharacterSprite(string name, CharacterSprite sprite)
		{
			List<CharacterSprite> characterSprites;
			bool exist = CharactersSprites.TryGetValue(name, out characterSprites);

			if (!exist)
			{
				characterSprites = new List<CharacterSprite>();
				CharactersSprites[name] = characterSprites;
			}

			characterSprites.Add(sprite);
		}

		void Start()
		{
			CharactersSprites = new Dictionary<string, List<CharacterSprite>>();

			foreach(SpriteItem item in _spriteItems)
			{
				if (string.IsNullOrWhiteSpace(item.Name))
					throw new ArgumentException("Имя картинки персонажа не может быть пустым.");

				string[] words = item.Name.Split(" ");

				string name = words[0];
				string details = words.Skip(1).ToWord();

				CharacterSprite sprite = new CharacterSprite(details, item.sprite);
				SetCharacterSprite(name, sprite);
			}
		}
	}
}
