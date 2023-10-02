using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core.Functions
{
	[PyImport("config")]
	public static class GameConfig
	{
		public static int ScreenWidth { get; set; }

		[PyImport("debug_log")]
		public static object DebugLog(int value)
		{
			Console.WriteLine(value);
			return null;
		}
	}
}
