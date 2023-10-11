using UnityEngine;

namespace Assets.Scripts.RenSharpClient.Models
{
	internal class CharacterImage
	{
		internal string Details { get; set; }
		internal Sprite Sprite { get; set; }

		public CharacterImage(string details, Sprite sprite)
		{
			Details = details;
			Sprite = sprite;
		}	
	}
}
