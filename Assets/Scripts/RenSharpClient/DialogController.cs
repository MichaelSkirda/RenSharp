using RenSharp.Models;
using RenSharp.Models.Commands;
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
	public void SetText(string text, string character = "")
	{
		TryStopCoroutine();
		DisplayText = text;
	}
	public void SetMessage(MessageResult message)
    {
        Message = message;
	}
    
	public void DrawText(bool clear)
    {
		TryStopCoroutine();
		Text = "";

		if(clear)
			ClearDialog();

        Coroutine = AnimateText();
        StartCoroutine(Coroutine);
	}

	public void DrawText(MessageResult message)
	{
		Message = message;
		DrawText(clear: true);
	}

	public void AppendText(MessageResult message)
	{
		Message = message;
		DrawText(clear: false);
	}

	public void SetCharacterName(string name)
	{
		NameField.text = name;
	}

	private IEnumerator AnimateText()
    {
		string message = Message.Speech;
		for(int i = 0; i < message.Length; i++)
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
	}

	private void TryStopCoroutine()
	{
		if (Coroutine != null)
			StopCoroutine(Coroutine);
	}

}

