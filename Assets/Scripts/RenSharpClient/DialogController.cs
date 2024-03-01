using RenSharp.Models;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogController : MonoBehaviour
{

    public TextMeshProUGUI TextField;
	public TextMeshProUGUI NameField;

	private MessageResult Message { get; set; }
    private string DisplayText { get; set; }
	private string Text { get; set; }

    private IEnumerator Coroutine { get; set; }

    void Start() => ClearDialog();
	
	void Update() => TextField.text = DisplayText;
	

	public bool HasAnimationFinished()
	{
		if (Message == null)
			return true;
		if (Text == Message.Speech)
			return true;
		Debug.Log("Not finished!");
		return false;
	}

	public void SkipAnimation()
	{
		TryStopCoroutine();

		if (Message == null)
			return;

		try
		{
			int drawedTextLength = Text.Length;
			string notDrawedText = Message.Speech.Substring(drawedTextLength);

			DisplayText += notDrawedText;
			Text = Message.Speech;
		}
		catch
		{
			Debug.LogError("CANNOT SKIP ANIMATION");
			DisplayText = Message.Speech;
			Text = Message.Speech;
		}
		
	}

	public void ClearDialog()
	{
		DisplayText = string.Empty;
	}


    public void DrawText(MessageResult message, float delay, Action callback)
    {
        Message = message;
        DrawText(clear: true, delay, callback);
    }

    public void DrawText(bool clear, float delay, Action callback)
    {
		TryStopCoroutine();
		Text = "";

		if(clear)
			ClearDialog();

        Coroutine = AnimateText(callback, delay);
        StartCoroutine(Coroutine);
	}
	
	public void SetCharacterName(string name)
	{
		NameField.text = name;
	}

	private IEnumerator AnimateText(Action callback, float callbackDelay)
	{

		string message = Message.Speech;
		for (int i = 0; i < message.Length; i++)
		{
			char chr = message[i];

			// Draw tag without animation
			/*if(chr == '<')
            {
                DisplayText += chr;
                Text += chr;
                int attempt = 0;
                try
                {
                    do
                    {
                        i++;
                        attempt++;
                        chr = message[i];
                        DisplayText += chr;
                        Text += chr;
                        if (attempt > 20)
                            throw new System.Exception("Too many atempts");
                    } while (chr != '>');
                    continue;
                }
                catch
                {
                    Debug.LogError("CANNOT DRAW TAG!!!");
                    SkipAnimation();
                    yield break;
                }
            } */

			// milliseconds to seconds
			// TODO
			float delay = 30 / 1000f;
			yield return new WaitForSeconds(delay);

			DisplayText += chr;
			Text += chr;
		}

		if(callback != null)
		{
			if(callbackDelay > 0)
				yield return new WaitForSeconds(callbackDelay);
            callback.Invoke();
        }
        
    }

	private void TryStopCoroutine()
	{
		if (Coroutine != null)
			StopCoroutine(Coroutine);
	}

}

