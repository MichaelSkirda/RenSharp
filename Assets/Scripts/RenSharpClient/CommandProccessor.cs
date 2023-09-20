using Assets.Scripts;
using RenSharp.Core;
using RenSharp.Models;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CommandProccessor : MonoBehaviour
{
	[SerializeField]
	public TextAsset RenSharpCode;
	public DialogController Dialog;

    private RenSharpCore RenSharp { get; set; }
	public bool IsPaused { get; set; } = false;

	void Start()
    {
		string[] lines = RenSharpCode.text.Split('\n');

		RenSharp = new RenSharpCore(lines);
		DialogWriter writer = new DialogWriter(Dialog);
		RenSharp.Writer = writer;

		MethodInfo method = this.GetType().GetMethod("CallbackTest");
		RenSharp.RegisterCallback("testCallback", method, this);
	}

	public void CallbackTest()
	{
        Transform child = transform.GetChild(0);
		child.position += new Vector3(0, 20);
	}

	void Update()
    {
		if (IsPaused)
			return;

		if (Input.GetKeyDown(KeyCode.Mouse0) == false && Input.GetKeyDown(KeyCode.Space) == false)
			return;

		if (Dialog.HasAnimationFinished() == false)
		{
			Dialog.SkipAnimation();
			return;
		}

		Command command = RenSharp.ReadNext();
	}
}
