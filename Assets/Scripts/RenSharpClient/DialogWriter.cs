using RenSharp.Interfaces;
using RenSharp.Models;
using System;

internal class DialogWriter : IWriter
{
	private DialogController Dialog { get; set; }

	public DialogWriter(DialogController dialog)
	{ 
		Dialog = dialog;
	}

    public void Write(MessageResult message)
        => Write(message, delay: 0, callback: null);

    public void Write(MessageResult message, float delay, Action callback)
    {
        string name = message.Attributes.GetValueOrNull("name");

        if (name == null || name == "_rs_nobody_name")
            name = "";

        Dialog.DrawText(message, delay, callback);
        Dialog.SetCharacterName(name);
    }
}

