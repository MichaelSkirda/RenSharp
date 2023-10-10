using Assets.Scripts.RenSharpClient.Commands;
using RenSharp;
using System.Collections.Generic;
using System.Linq;

internal static class CommandParsers
{

	internal static Show ParseShow(string[] words, ImageController controller)
	{
		string name = words[1];
		string details = words.Skip(2).ToWord();
		var attributes = new List<string>();
		return new Show(name, details, attributes, controller);
	}

}
