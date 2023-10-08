using Assets.Scripts;
using RenSharp.Core;
using RenSharp.Models;
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
