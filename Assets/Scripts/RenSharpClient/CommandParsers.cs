
using Assets.Scripts.RenSharpClient.Commands;
using UnityEngine;

internal static class CommandParsers
{

	internal static Show ParseShow(string[] words)
	{

		return new Show(null, null, null, null);
	}

}
