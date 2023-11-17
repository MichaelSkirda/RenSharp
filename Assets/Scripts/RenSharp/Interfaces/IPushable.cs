using RenSharp.Core;

namespace RenSharp.Interfaces
{
	internal interface IPushable
	{
		// Метод Execute у Command решает надо ли испльзовать Push.
		// Метод Push решает какое значение надо пушить.
		bool Push(RenSharpContext ctx);
	}
}
