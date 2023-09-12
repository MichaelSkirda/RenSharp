


using RenSharp;
using RenSharp.Models;
using RenSharpClient;

internal class Program
{
	private static void Main(string[] args)
	{
		string path = "./test.csren";
		Writer writer = new Writer();
		RenSharpCore renSharp = new RenSharpCore(path); 
		renSharp.Writer = writer;

		while(true)
		{
			Command command = renSharp.ReadNext();
			Console.ReadKey();
		}
	}
}