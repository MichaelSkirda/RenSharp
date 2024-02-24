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

	private DateTime LastRollback { get; set; } = DateTime.Now;
	private DateTime LastFastForward { get; set; } = DateTime.Now;
	private int RollbackCooldown { get; set; }
	private int FastForwardDelay { get; set; }

	void Start()
    {
		string[] lines = RenSharpCode.text.Split('\n');
		Configuration config = UnityConfigDefault.GetDefault(ImageController, SoundController, MenuController);
		RollbackCooldown = config.GetValueOrDefault<int>("rollback_cooldown");
		FastForwardDelay = config.GetValueOrDefault<int>("fast_forward_delay");

		DialogWriter writer = new DialogWriter(Dialog);
		config.Writer = writer;

		RenSharp = new RenSharpCore(lines, config);

        string save = PlayerPrefs.GetString("save1");
		if(string.IsNullOrWhiteSpace(save) == false)
		{
            RenSharp.Load(save);
            Debug.Log("Loaded on start!");
        }
		else
		{
            Debug.Log("No save");
        }
    }

	void Update()
    {
		if(Input.GetKey(KeyCode.LeftControl))
		{
			TryReadNext();
		}
		else if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
		{
			ReadNext();
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.LeftArrow)) // backwards
		{
			TryRollback();
		}
		else if(Input.GetKeyDown(KeyCode.Mouse1))
		{
			string serialized = RenSharp.Save();
			DateTime start = DateTime.Now;
			Debug.Log(serialized);
			double delta = (DateTime.Now - start).TotalMilliseconds;
			Debug.Log("Delta time: " + delta);
		}
		else if(Input.GetKeyDown(KeyCode.LeftShift))
		{
            string serialized = RenSharp.Save();
			PlayerPrefs.SetString("save1", serialized);
			Debug.Log("Saved!");
		}
		else if(Input.GetKeyDown(KeyCode.Tab))
		{
			string save = PlayerPrefs.GetString("save1");
            RenSharp.Load(save);
            Debug.Log("Loaded!");
        }
		else if(Input.GetKeyDown(KeyCode.Delete))
		{
			PlayerPrefs.DeleteKey("save1");
			Debug.Log("Deleted!");
		}
    }

	private void TryRollback()
	{
		if ((DateTime.Now - LastRollback).Milliseconds < RollbackCooldown)
			return;

		bool hasRollback = RenSharp.Rollback();
		if (hasRollback == false)
			Debug.Log("No rollback.");
	}

	private void TryReadNext()
	{
		if ((DateTime.Now - LastFastForward).Milliseconds < RollbackCooldown)
			return;
		ReadNext();
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
