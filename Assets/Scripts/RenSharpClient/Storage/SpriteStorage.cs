using Assets.Scripts.RenSharpClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RenSharp;
using System.IO;

namespace RenSharpClient.Storage
{
	internal class SpriteStorage : MonoBehaviour
	{
		[SerializeField]
		private SpriteItem[] _spriteItems;
		private Dictionary<string, List<RenSharpImage>> CharactersSprites { get; set; }

		public RenSharpImage GetSprite(string name, string details)
		{
			List<RenSharpImage> characterSprites;

			bool exist = CharactersSprites.TryGetValue(name, out characterSprites);
			if(!exist)
				throw new FileNotFoundException($"Не получилось найти спрайт персонажа '{name} {details}'");

			RenSharpImage sprite = characterSprites.FirstOrDefault(x => x.Details == details);
			if(sprite == null)
				throw new FileNotFoundException($"Не получилось найти спрайт персонажа '{name} {details}'");

			return sprite;
		}

		public IEnumerable<RenSharpImage> GetSprites(string name)
		{
			List<RenSharpImage> characterSprites;

			bool notExist = !CharactersSprites.TryGetValue(name, out characterSprites);
			if (notExist)
				throw new FileNotFoundException($"Не получилось найти спрайт персонажа '{name}'");

			return characterSprites;
		}

		private void SetSprite(string name, RenSharpImage sprite)
		{
			List<RenSharpImage> characterSprites;
			bool exist = CharactersSprites.TryGetValue(name, out characterSprites);

			if (!exist)
			{
				characterSprites = new List<RenSharpImage>();
				CharactersSprites[name] = characterSprites;
			}

			characterSprites.Add(sprite);
		}


		void Start()
		{
			CharactersSprites = new Dictionary<string, List<RenSharpImage>>();

			foreach(SpriteItem item in _spriteItems)
			{
				if (string.IsNullOrWhiteSpace(item.Name))
					throw new ArgumentException("Имя картинки персонажа не может быть пустым.");

				string[] words = item.Name.Split(" ");

				string name = words[0];
				string details = words.Skip(1).ToWord();

				Sprite sprite = item.Sprite;
				float height = sprite.rect.height;
				float width = sprite.rect.width;

				RenSharpImage image = new RenSharpImage(details, item.Sprite, width, height);
				SetSprite(name, image);
			}
		}
	}
}
