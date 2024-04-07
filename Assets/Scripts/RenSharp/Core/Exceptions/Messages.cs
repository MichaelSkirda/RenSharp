using System.Collections.Generic;

namespace RenSharp.Core.Exceptions
{
    internal class Messages
	{
		internal static string ReapitingLabelNames(IEnumerable<string> names)
			=> $"Лейбл(ы) с именем ({string.Join(", ", names)}) повторяются. Имя лейбла должно быть уникальным или переопределять системное.";
		internal static string LabelNotExist(string label)
			=> $"Лейбл с именем '{label}' не существует.";
	}
}
