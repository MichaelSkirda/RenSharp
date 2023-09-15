﻿using RenSharp.Interfaces;
using RenSharp;
using System.Collections.Generic;
using RenSharp.Models.Commands;

namespace Assets.Scripts
{
	internal class DialogWriter : IWriter
	{
		private DialogController Dialog { get; set; }

		public DialogWriter(DialogController dialog)
		{ 
			Dialog = dialog;
		}

		public void Write(Message message)
		{
			string name = message.Character;

			if (name == null)
				name = "";

			Dialog.DrawText(message);
			Dialog.SetCharacterName(name);
		}
	}
}