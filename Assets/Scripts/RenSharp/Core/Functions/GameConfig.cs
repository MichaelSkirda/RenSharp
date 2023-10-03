using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core.Functions
{
	[PyImport("config")]
	public static class GameConfig
	{
		public static int ScreenWidth { get; set; } = 1024;

		[PyImport("debug_log")]
		public static object DebugLog(string value)
		{
			Console.WriteLine(value);
			return null;
		}
	}
}
