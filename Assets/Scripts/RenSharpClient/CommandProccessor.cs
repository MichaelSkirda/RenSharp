using Assets.Scripts;
using Assets.Scripts.RenSharpClient.Controllers;
using RenSharp;
using RenSharp.Core;
using RenSharp.Models;
using UnityEngine;

public class CommandProccessor : MonoBehaviour
{
	[SerializeField]
	public TextAsset RenSharpCode;
	public DialogController Dialog;

	[SerializeField]
	private ImageController ImageController;
	[SerializeField]
	private SoundController SoundController;

	private RenSharpCore RenSharp { get; set; }
	public bool IsPaused { get; set; } = false;

	void Start()
    {
		string[] lines = RenSharpCode.text.Split('\n');
		Configuration config = RSUnityConfig.GetDefault(ImageController, SoundController);

		DialogWriter writer = new DialogWriter(Dialog);
		config.Writer = writer;

		RenSharp = new RenSharpCore(lines, config);
	}

	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Mouse0) == false && Input.GetKeyDown(KeyCode.Space) == false)
			return;

		ReadNext();
	}

	public void ReadNext()
	{
		if (IsPaused)
			return;

		if (Dialog.HasAnimationFinished() == false)
		{
			Dialog.SkipAnimation();
			return;
		}

		Command command = RenSharp.ReadNext();
	}
}
