using Assets.Scripts.RenSharpClient.Models;
using RenSharp;
using RenSharp.Core;
using RenSharpClient.Commands.Results;
using RenSharpClient.Effects;
using RenSharpClient.Models;
using RenSharpClient.Models.Commands.Results;
using RenSharpClient.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RenSharpClient.Controllers
{
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

		private Dictionary<string, ActiveSprite> ActiveSprites { get; set; }

		private void Start()
		{
			ActiveSprites = new Dictionary<string, ActiveSprite>();
		}

		internal IEnumerable<ActiveSprite> GetActiveSprites()
			=> ActiveSprites.Values;

		internal void Show(ShowResult show, Configuration config, RenSharpCore core)
		{
			ActiveSprite activeSprite;
			bool isExist = ActiveSprites.TryGetValue(show.Name, out activeSprite);

			if (!isExist)
			{
				activeSprite = new ActiveSprite()
				{
					Name = show.Name,
					Details = show.Details,
					Attributes = show.attributes
				};
				activeSprite.obj = Instantiate(SpritePrefab, Parent.transform);
			}

			RenSharpImage toSet;

			if (string.IsNullOrWhiteSpace(show.Details))
				toSet = Sprites.GetSprite(show.Name);
			else
				toSet = Sprites.GetSprite(show.Name, show.Details);

			RectTransform rect = activeSprite.obj.GetComponent<RectTransform>();

			rect.anchorMin = new Vector2(0.5f, 0f);
			rect.anchorMax = new Vector2(0.5f, 0f);

			rect.sizeDelta = GetSize(show, toSet, config);

			float x = GetX(show);
			float y = rect.rect.height / 2;

			rect.anchoredPosition = new Vector2(x, y);


			Image image = activeSprite.obj.GetComponent<Image>();
			image.sprite = toSet.Sprite;
			ActiveSprites[show.Name] = activeSprite;

			string effectMethod = show.attributes.GetValueOrNull("with");
			var effect = core.Context.Evaluate<Func<Image, EffectData, IEnumerator>>(effectMethod);
			var effectData = new EffectData() { IsAppear = true };
			StartCoroutine(effect(image, effectData));
		}

		private Vector2 GetSize(ShowResult show, RenSharpImage image, Configuration config)
		{
			Rect rect = image.Sprite.rect;
			string fullscreen = show.attributes["fullscreen"];
			float multiplier;


			// ����������� �� ������ � ������
			// ���� fullscreen �� ������� ���������� ������� ������
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
			ActiveSprite character;
			ActiveSprites.TryGetValue(name, out character);

			if (character == null)
				return;

			Destroy(character.obj);
			ActiveSprites.Remove(name);
		}

		internal void HideAll()
		{
			foreach (KeyValuePair<string, ActiveSprite> character in ActiveSprites.ToList())
			{
				string name = character.Key;
				GameObject obj = character.Value.obj;

				Destroy(obj);
				ActiveSprites.Remove(name);
			}
		}

		internal void SetSize(ImageResult imageResult)
		{
			IEnumerable<RenSharpImage> images;

			string widthStr = imageResult.Width;
			string heightStr = imageResult.Height;

			float zoom = imageResult.Zoom;

			float width;
			float height;

			bool hasWidth = float.TryParse(widthStr, out width);
			bool hasHeight = float.TryParse(heightStr, out height);

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

			if (heightStr == "auto")
			{
				if (hasWidth == false)
					throw new ArgumentException("�������� width ������ ����� �������� ��� height auto");
				ResizeHeightAuto(images, width, zoom);
			}
			else if (widthStr == "auto")
			{
				if (hasHeight == false)
					throw new ArgumentException("�������� height ������ ����� �������� ��� width auto");
				ResizeWidthAuto(images, height, zoom);
			}
			else
			{
				RenSharpImage image = images.FirstOrDefault();
				if (image == null)
					throw new ArgumentException($"�������� � ������ '{name} {details}' �� ������.");
				if (hasHeight == false)
					height = image.Height;
				if (hasWidth == false)
					width = image.Width;
				ResizeDefault(images, width, height, zoom);
			}
		}

		private void ResizeDefault(IEnumerable<RenSharpImage> images, float width, float height, float zoom)
		{
			foreach (RenSharpImage image in images)
			{
				image.Height = height * zoom;
				image.Width = width * zoom;
			}
		}

		private void ResizeHeightAuto(IEnumerable<RenSharpImage> images, float width, float zoom)
		{
			foreach (RenSharpImage image in images)
			{
				width *= zoom;
				float multiplier = width / image.Width;

				image.Width = width;
				image.Height *= multiplier;
			}
		}

		private void ResizeWidthAuto(IEnumerable<RenSharpImage> images, float height, float zoom)
		{
			foreach (RenSharpImage image in images)
			{
				height *= zoom;
				float multiplier = height / image.Height;

				image.Width *= multiplier;
				image.Height = height;
			}
		}
	}
}