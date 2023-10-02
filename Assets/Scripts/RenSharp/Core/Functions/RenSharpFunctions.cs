using System;

namespace RenSharp.Core.Functions
{
    public static class RenSharpFunctions
    {
        [PyImport] public static int Random(int min, int max) => new Random().Next(min, max);
        [PyImport] public static string Time() => DateTime.Now.ToString();
        [PyImport] public static string TimeF(string format) => DateTime.Now.ToString(format);
        [PyImport]
        public static object Print(string line)
        {
            Console.WriteLine(line);
            return null;
        }
        [PyImport] public static string GetEnv(string env) => Environment.GetEnvironmentVariable(env);
    }
}
