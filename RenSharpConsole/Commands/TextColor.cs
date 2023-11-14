using RenSharp.Core;
using RenSharp.Models;

namespace RenSharpConsole.Commands
{
	public class TextColor : Command
	{
		string ColorCode { get; set; }
		IFormatter Formatter { get; set; }

		public TextColor(string colorCode, IFormatter formatter)
		{
			ColorCode = colorCode;
			Formatter = formatter;
		}

		public override void Execute(RenSharpCore core)
		{
			Formatter.SetFormat(ColorCode);
		}

		public override Command Rollback()
		{
			string currentColor = Formatter.GetCurrentColor();
			TextColor command = new TextColor(currentColor, Formatter);

			command.Line = Line;
			command.SourceLine = SourceLine;
			command.Level = Level;

			return command;
		}
	}
}
