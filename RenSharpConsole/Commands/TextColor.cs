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
	}
}
