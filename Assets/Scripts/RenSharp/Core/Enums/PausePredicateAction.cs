using System;

namespace RenSharp.Core.Enums
{
	[Flags]
	public enum PausePredicateAction
	{
		Pause = 1,
		Unpause = 2,
		NoChange = 4,
		Unsubscribe = 8
	}
}
