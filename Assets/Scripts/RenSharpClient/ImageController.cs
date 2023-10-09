using Assets.Scripts.RenSharpClient;
using Assets.Scripts.RenSharpClient.Commands.Results;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageController : MonoBehaviour
{
	[SerializeField]
	private DataStorage Data;
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
		GameObject character;
		bool isExist = Characters.TryGetValue(name, out character);

		if (!isExist)
			character = Instantiate(CharacterPrefab, Parent.transform);


		Characters[name] = character;
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
