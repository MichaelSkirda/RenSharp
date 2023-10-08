using RenSharp.Interfaces;
using RenSharp.Models;

namespace RenSharpConsole
{
	internal class ConsoleWriter : IWriter
	{
		public void Write(MessageResult message)
		{
			string speaker = message.Attributes.GetSpeaker();
			string speech = message.Speech;

			Console.WriteLine($"{speaker}: {speech}");
        }
	}
}
