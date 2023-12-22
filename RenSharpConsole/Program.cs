using RenSharp;
using RenSharp.Core;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharpConsole;

internal class Program
{
	private static void Main(string[] args)
	{
		IFormatter formatter = new OutputFormatter();
		IWriter writer = new ConsoleWriter(formatter);
		Configuration config = ConsoleConfig.GetDefaultConfig(formatter, writer);

		string path = "./test.csren";
		var renSharp = new RenSharpCore(path, config);

		renSharp.ReadNext();

		while (true)
		{
			if (Console.ReadKey().Key == ConsoleKey.LeftArrow)
			{
				try
				{
					renSharp.Save();
					renSharp.Rollback();
				}
				catch
				{
					Console.WriteLine("Rollback пустой.");
					continue;
				}
			}
			else
			{
				renSharp.ReadNext();
			}
		}
	}

}