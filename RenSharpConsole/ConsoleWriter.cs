using RenSharp.Interfaces;
using RenSharp.Models;

namespace RenSharpConsole
{
	internal class ConsoleWriter : IWriter
	{
		private IFormatter Formatter { get; set; }

		public ConsoleWriter(IFormatter formatter)
		{
			Formatter = formatter;
		}

		public void Write(MessageResult message)
		{
			string speaker = message.Attributes.GetSpeaker();
			string speech = message.Speech;

			speaker = Formatter.FormatDefault(speaker);
			speech = Formatter.Format(speech);

			Console.WriteLine($"{speaker}: {speech}");
        }

		public void Write(MessageResult message, float delay, Action Callback)
		{
			Write(message);
			Callback();
		}
	}
}
