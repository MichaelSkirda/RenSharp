using Assets.Scripts.RenSharpClient;
using Assets.Scripts.RenSharpClient.Commands.Results;
using Assets.Scripts.RenSharpClient.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{
	[SerializeField]
	private SpriteStorage Data;
	[SerializeField]
	private GameObject CharacterPrefab;
	[SerializeField]
	private GameObject Parent;

	private Dictionary<string, GameObject> Characters { get; set; }

	private void Start()
	{
		Characters = new Dictionary<string, GameObject>();
	}

	internal void ShowCharacter(ShowResult show)
	{
		GameObject obj;
		bool isExist = Characters.TryGetValue(show.Name, out obj);

		if (!isExist)
			obj = Instantiate(CharacterPrefab, Parent.transform);

		CharacterImage toSet = Data.GetCharacterSprite(show.Name, show.Details);
		RectTransform rect = obj.GetComponent<RectTransform>();
		float multiplier = 900 / toSet.Sprite.rect.height;

		rect.anchorMin = new Vector2(0.5f, 0f);
		rect.anchorMax = new Vector2(0.5f, 0f);

		rect.sizeDelta = new Vector2(toSet.Sprite.rect.width * multiplier, toSet.Sprite.rect.height * multiplier);
		rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.rect.height / 2);


		Image characterImage = obj.GetComponent<Image>();
		characterImage.sprite = toSet.Sprite;
		Characters[show.Name] = obj;
	}

	internal void HideCharacter(string name)
	{
		GameObject character;
		Characters.TryGetValue(name, out character);

		if (character == null)
			return;

		Destroy(character);
		Characters.Remove(name);
	}
}
