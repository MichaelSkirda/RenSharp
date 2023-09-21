using RenSharp.Interfaces;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharpConsole
{
	internal class ConsoleWriter : IWriter
	{
		public void Write(Message message)
		{
			string speaker = message.Attributes.GetSpeaker();
			string speech = message.Speech;

			Console.WriteLine($"{speaker}: {speech}");
        }
	}
}
