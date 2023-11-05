using Assets.Scripts.RenSharpClient;
using Assets.Scripts.RenSharpClient.Commands.Results;
using Assets.Scripts.RenSharpClient.Models;
using Assets.Scripts.RenSharpClient.Storage;
using RenSharp;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

		int screenHeight = config.GetValueOrDefault<int>("screen_height");
		int screenWidth = config.GetValueOrDefault<int>("screen_width");

		// –аст€гивает по высоте и ширине
		// ≈сли fullscreen не указано использует обычный размер
		if (fullscreen == "height")
			multiplier = screenHeight / rect.height; 
		else if (fullscreen == "width" || fullscreen == string.Empty)
			multiplier = screenWidth / rect.width;
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

	internal void SetSize(string name, string details, int? width, int? height)
	{
		bool hasDetails = !string.IsNullOrWhiteSpace(details);

		if (hasDetails)
			SetSizeSignle(name, details, width, height);
		else
			SetSizeAll(name, width, height);
	}

	internal void SetSizeSignle(string name, string details, int? width, int? height)
	{
		RenSharpImage image = Sprites.GetSprite(name, details);
		SetSize(image, width, height);
	}

	internal void SetSizeAll(string name, int? width, int? height)
	{
		IEnumerable<RenSharpImage> images = Sprites.GetSprites(name);

		foreach (RenSharpImage image in images)
		{
			SetSize(image, width, height);
		}
	}

	internal void SetSize(RenSharpImage image, int? width, int? height)
	{
		if (height != null)
			image.Height = height.Value;
		if (width != null)
			image.Width = width.Value;
	}

}
