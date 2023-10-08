using System;

namespace RenSharp.Core.Functions
{
	[PyImport("rs")]
	public static class RenSharpFunctions
    {
        public static int Random(int min, int max) => new Random().Next(min, max);
        public static string Time() => DateTime.Now.ToString();
        public static string TimeF(string format) => DateTime.Now.ToString(format);
        public static object Print(string line)
        {
            Console.WriteLine(line);
            return null;
        }
        public static string GetEnv(string env) => Environment.GetEnvironmentVariable(env);
    }
}
