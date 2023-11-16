using RenSharp.Core;

namespace RenSharp.Models.Commands
{
	/// <summary>
	/// Вы можете переопределить этот класс, чтобы указать свое поведение команды Exit, наример,
	/// она может вести в меню, скрывать диалоговое окно или вызывать любой другой метод.
	/// </summary>
	public class Exit : Command
	{
		public override void Execute(RenSharpCore core)
		{
			
		}
	}
}
