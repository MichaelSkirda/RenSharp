using UnityEngine;

namespace RenSharpClient.Models
{
	internal class RenSharpImage
	{
		internal string Details { get; set; }
		internal Sprite Sprite { get; set; }
		internal float Width { get; set; }
		internal float Height { get; set; }

		public RenSharpImage(string details, Sprite sprite, float width, float height)
		{
			Details = details;
			Sprite = sprite;
			Width = width;
			Height = height;
		}
	}
}
