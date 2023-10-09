
using UnityEngine;

namespace Assets.Scripts.RenSharpClient.Models
{
	internal class CharacterSprite
	{
		internal string Details { get; set; }
		internal Sprite Sprite { get; set; }

		public CharacterSprite(string details, Sprite sprite)
		{
			Details = details;
			Sprite = sprite;
		}	
	}
}
