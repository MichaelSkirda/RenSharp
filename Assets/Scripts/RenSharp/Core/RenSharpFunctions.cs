using System;

namespace RenSharp.Core
{
	public static class RenSharpFunctions
	{
		[Callback] public static int Random(int min, int max) => new Random().Next(min, max);
		[Callback] public static string Time() => DateTime.Now.ToString();
		[Callback] public static string TimeF(string format) => DateTime.Now.ToString(format);
		[Callback] public static object Print(string line)
		{
			Console.WriteLine(line);
			UnityEngine.Debug.Log(line);
			return null;
		}
		[Callback] public static string GetEnv(string env) => Environment.GetEnvironmentVariable(env);
	}
}
