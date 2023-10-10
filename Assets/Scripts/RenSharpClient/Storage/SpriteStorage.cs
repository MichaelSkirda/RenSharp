using Assets.Scripts.RenSharpClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RenSharp;
using Assets.Scripts.RenSharpClient.Commands;
using System.IO;

namespace Assets.Scripts.RenSharpClient
{
	internal class SpriteStorage : MonoBehaviour
	{
		[SerializeField]
		private SpriteItem[] _spriteItems;
		internal Dictionary<string, List<CharacterImage>> CharactersSprites { get; set; }

		public CharacterImage GetCharacterSprite(string name, string details)
		{
			List<CharacterImage> characterSprites;

			bool exist = CharactersSprites.TryGetValue(name, out characterSprites);
			if(!exist)
				throw new FileNotFoundException($"Не получилось найти спрайт персонажа '{name} {details}'");

			CharacterImage sprite = characterSprites.FirstOrDefault(x => x.Details == details);
			if(sprite == null)
				throw new FileNotFoundException($"Не получилось найти спрайт персонажа '{name} {details}'");

			return sprite;
		}

		private void SetCharacterSprite(string name, CharacterImage sprite)
		{
			List<CharacterImage> characterSprites;
			bool exist = CharactersSprites.TryGetValue(name, out characterSprites);

			if (!exist)
			{
				characterSprites = new List<CharacterImage>();
				CharactersSprites[name] = characterSprites;
			}

			characterSprites.Add(sprite);
		}

		void Start()
		{
			CharactersSprites = new Dictionary<string, List<CharacterImage>>();

			foreach(SpriteItem item in _spriteItems)
			{
				if (string.IsNullOrWhiteSpace(item.Name))
					throw new ArgumentException("Имя картинки персонажа не может быть пустым.");

				string[] words = item.Name.Split(" ");

				string name = words[0];
				string details = words.Skip(1).ToWord();

				CharacterImage sprite = new CharacterImage(details, item.Sprite);
				SetCharacterSprite(name, sprite);
			}
		}
	}
}
