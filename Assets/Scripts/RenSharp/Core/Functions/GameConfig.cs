using System;

namespace RenSharp.Core.Functions
{
	[PyImport("config")]
	public static class GameConfig
	{
		public static int ScreenWidth { get; set; } = 1920;
		public static int ScreenHeight = 1080;

		[PyImport("debug_log")]
		public static void DebugLog(object value)
		{
			Console.WriteLine(value);
		}

		public static void WillImport(object someval)
		{
            Console.WriteLine(someval);
        }
	}
}
