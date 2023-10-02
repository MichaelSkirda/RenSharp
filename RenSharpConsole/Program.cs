using Flee.PublicTypes;
using RenSharp;
using RenSharp.Core;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharpConsole;

internal class Program
{
	private static void Main(string[] args)
	{
		string path = "./test.csren";
		IWriter writer = new ConsoleWriter();
		RenSharpCore renSharp = new RenSharpCore(path, new Configuration() { Writer = writer });
		renSharp.Writer = writer;

		while (true)
		{
			Command command = renSharp.ReadNext();
			Console.ReadKey();
		}
	}
}