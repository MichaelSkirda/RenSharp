using Flee.PublicTypes;
using RenSharp.Core;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharpConsole;

internal class Program
{
	private static void Main(string[] args)
	{
		string path = "./test.txt";
		IWriter writer = new ConsoleWriter();
		RenSharpCore renSharp = new RenSharpCore(path);
		renSharp.Writer = writer;

		while (true)
		{
			Command command = renSharp.ReadNext();
			Console.ReadKey();
		}
	}
}