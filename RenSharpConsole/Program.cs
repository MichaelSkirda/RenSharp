using RenSharp;
using RenSharp.Core;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharpConsole;

internal class Program
{
	private static void Main(string[] args)
	{
		Configuration config = ConsoleConfig.GetDefaultConfig();

		string path = "./test.csren";
		var renSharp = new RenSharpCore(path, config);

		renSharp.ReadNext();
		WriteMessages(renSharp);

		while (true)
		{
			if (Console.ReadKey().Key == ConsoleKey.LeftArrow)
			{
				try
				{
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

			WriteMessages(renSharp);
		}
	}

	private static void WriteMessages(RenSharpCore renSharp)
	{
		Console.Clear();
		foreach (MessageResult message in renSharp.Context.MessageHistory.All())
		{
			Console.WriteLine("-------------------");
			Console.WriteLine($"{message.Character}: {message.Speech}");
		}
		Console.WriteLine("-------------------");
	}
}