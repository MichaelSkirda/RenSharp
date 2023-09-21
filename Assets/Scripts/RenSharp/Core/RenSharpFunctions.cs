using RenSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core
{
	public static class RenSharpFunctions
	{
		[Callback] public static int Random(int min, int max) => new Random().Next(min, max);
		[Callback] public static string Time(string format = "") => DateTime.Now.ToString(format);
	}
}
