using Assets.Scripts.RenSharpClient;
using Assets.Scripts.RenSharpClient.Commands.Results;
using Assets.Scripts.RenSharpClient.Models;
using Assets.Scripts.RenSharpClient.Storage;
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
	private GameObject CharacterPrefab;
	[SerializeField]
	private GameObject Parent;

	private Dictionary<string, GameObject> Characters { get; set; }

	private void Start()
	{
		Characters = new Dictionary<string, GameObject>();
	}

	internal void Show(ShowResult show)
	{
		GameObject obj;
		bool isExist = Characters.TryGetValue(show.Name, out obj);

		if (!isExist)
			obj = Instantiate(CharacterPrefab, Parent.transform);

		CharacterImage toSet = Sprites.GetCharacterSprite(show.Name, show.Details);
		RectTransform rect = obj.GetComponent<RectTransform>();

		rect.anchorMin = new Vector2(0.5f, 0f);
		rect.anchorMax = new Vector2(0.5f, 0f);

		rect.sizeDelta = GetSize(toSet.Sprite.rect);

		float x = GetX(show);
		float y = rect.rect.height / 2;

		rect.anchoredPosition = new Vector2(x, y);


		Image characterImage = obj.GetComponent<Image>();
		characterImage.sprite = toSet.Sprite;
		Characters[show.Name] = obj;
	}

	private Vector2 GetSize(Rect rect)
	{
		float multiplier = 900 / rect.height;
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
		Characters.TryGetValue(name, out character);

		if (character == null)
			return;

		Destroy(character);
		Characters.Remove(name);
	}

	internal void HideAll()
	{
		foreach(KeyValuePair<string, GameObject> character in Characters)
		{
			string name = character.Key;
			GameObject obj = character.Value;

			Destroy(obj);
			Characters.Remove(name);
		}
	}

}
