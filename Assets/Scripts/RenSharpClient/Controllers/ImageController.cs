using Assets.Scripts.RenSharpClient;
using Assets.Scripts.RenSharpClient.Commands.Results;
using Assets.Scripts.RenSharpClient.Models;
using Assets.Scripts.RenSharpClient.Models.Commands.Results;
using Assets.Scripts.RenSharpClient.Storage;
using Microsoft.Cci;
using RenSharp;
using RenSharp.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static IronPython.Modules._ast;

public class ImageController : MonoBehaviour
{
	[SerializeField]
	private SpriteStorage Sprites;
	[SerializeField]
	private PointStorage Points;
	[SerializeField]
	private GameObject SpritePrefab;
	[SerializeField]
	private GameObject Parent;

	private Dictionary<string, GameObject> ActiveSprites { get; set; }

	private void Start()
	{
		ActiveSprites = new Dictionary<string, GameObject>();
	}

	internal void Show(ShowResult show, Configuration config)
	{
		GameObject obj;
		bool isExist = ActiveSprites.TryGetValue(show.Name, out obj);

		if (!isExist)
			obj = Instantiate(SpritePrefab, Parent.transform);

		RenSharpImage toSet = Sprites.GetSprite(show.Name, show.Details);
		RectTransform rect = obj.GetComponent<RectTransform>();

		rect.anchorMin = new Vector2(0.5f, 0f);
		rect.anchorMax = new Vector2(0.5f, 0f);

		rect.sizeDelta = GetSize(show, toSet, config);

		float x = GetX(show);
		float y = rect.rect.height / 2;

		rect.anchoredPosition = new Vector2(x, y);


		Image characterImage = obj.GetComponent<Image>();
		characterImage.sprite = toSet.Sprite;
		ActiveSprites[show.Name] = obj;
	}

	private Vector2 GetSize(ShowResult show, RenSharpImage image, Configuration config)
	{
		Rect rect = image.Sprite.rect;
		string fullscreen = show.attributes["fullscreen"];
		float multiplier;


		// –аст€гивает по высоте и ширине
		// ≈сли fullscreen не указано использует обычный размер
		if (fullscreen == "height")
		{
			int screenHeight = config.GetValueOrDefault<int>("screen_height");
			multiplier = screenHeight / rect.height;

		}
		else if (fullscreen == "width" || fullscreen == string.Empty)
		{
			int screenWidth = config.GetValueOrDefault<int>("screen_width");
			multiplier = screenWidth / rect.width;
		}
		else
		{
			float width = image.Width;
			float height = image.Height;

			return new Vector2(width, height);
		}

		return new Vector2(rect.width * multiplier, rect.height * multiplier);
	}

	private float GetX(ShowResult show)
	{
		string showAt = show.attributes["at"];
		RectTransform transform = Points.Find(showAt).Point;

		return transform.anchoredPosition.x;
	}

	internal void Hide(string name)
	{
		GameObject character;
		ActiveSprites.TryGetValue(name, out character);

		if (character == null)
			return;

		Destroy(character);
		ActiveSprites.Remove(name);
	}

	internal void HideAll()
	{
		foreach(KeyValuePair<string, GameObject> character in ActiveSprites)
		{
			string name = character.Key;
			GameObject obj = character.Value;

			Destroy(obj);
			ActiveSprites.Remove(name);
		}
	}

	internal void SetSize(ImageResult imageResult)
	{
		IEnumerable<RenSharpImage> images;

		int? height = imageResult.Height;
		int? width = imageResult.Width;
		float zoom = imageResult.Zoom;
		string name = imageResult.Name;
		string details = imageResult.Details;

		bool hasDetails = !string.IsNullOrWhiteSpace(details);

		if (hasDetails)
		{
			RenSharpImage image = Sprites.GetSprite(name, details);
			images = new List<RenSharpImage>() { image };
		}
		else
		{
			images = Sprites.GetSprites(name);
		}

		foreach (RenSharpImage image in images)
		{
			if (height != null)
				image.Height = height.Value * zoom;
			else
				image.Height *= zoom;

			if (width != null)
				image.Width = width.Value * zoom;
			else
				image.Width *= zoom;
		}
	}
}
