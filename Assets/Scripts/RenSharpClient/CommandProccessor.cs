using Assets.Scripts;
using RenSharp.Core;
using RenSharp.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandProccessor : MonoBehaviour
{
	[SerializeField]
	public TextAsset RenSharpCode;
	public DialogController dialogController;

    private RenSharpCore RenSharp { get; set; }
	private bool IsPaused { get; set; }

	void Start()
    {
		string[] lines = RenSharpCode.text.Split('\n');

		RenSharp = new RenSharpCore(lines);
		DialogWriter writer = new DialogWriter(dialogController);
		RenSharp.Writer = writer;
	}

	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Mouse0) == false && Input.GetKeyDown(KeyCode.Space) == false)
			return;

		Command command = RenSharp.ReadNext();
	}
}
