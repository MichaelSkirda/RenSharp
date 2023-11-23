using Assets.Scripts;
using RenSharp;
using RenSharp.Core;
using RenSharp.Core.Exceptions;
using RenSharp.Models;
using RenSharpClient.Controllers;
using System;
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
	[SerializeField]
	private MenuController MenuController;

	private RenSharpCore RenSharp { get; set; }
	public bool IsPaused { get; set; } = false;

	private DateTime LastRollback { get; set; }
	private int RollbackCooldown { get; set; }

	void Start()
    {
		string[] lines = RenSharpCode.text.Split('\n');
		Configuration config = UnityConfigDefault.GetDefault(ImageController, SoundController, MenuController);
		RollbackCooldown = config.GetValueOrDefault<int>("rollback_cooldown");

		DialogWriter writer = new DialogWriter(Dialog);
		config.Writer = writer;

		RenSharp = new RenSharpCore(lines, config);
	}

	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
			ReadNext();
		else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
			TryRollback();
	}

	private void TryRollback()
	{
		if ((DateTime.Now - LastRollback).Milliseconds < RollbackCooldown)
			return;

		bool hasRollback = RenSharp.Rollback();
		if (hasRollback == false)
			Debug.Log("No rollback.");
	}

	private void ReadNext()
	{
		try
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
		catch (RenSharpPausedException)
		{
			Debug.Log("Игра на паузе.");
		}
		catch (UnexpectedEndOfProgramException)
		{
			Debug.Log("Конец программы.");
		}
	}
}
