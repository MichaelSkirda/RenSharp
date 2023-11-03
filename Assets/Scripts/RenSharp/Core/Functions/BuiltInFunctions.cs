using System;

namespace RenSharp.Core.Functions
{
	[PyImport("RS")]
	public class BuiltInFunctions
	{
		private RenSharpCore Core { get; set; }
		private RenSharpContext Ctx => Core.Context;

		public BuiltInFunctions(RenSharpCore core)
		{
			Core = core;
		}

		[PyImport("clear_rollback")]
		public void ClearRollback() => Ctx.RollbackStack.Clear();

		#region static
		public static int Random(int min, int max) => new Random().Next(min, max);
		public static string Time() => DateTime.Now.ToString();
		public static string TimeF(string format) => DateTime.Now.ToString(format);
		public static void Print(string line) => Console.WriteLine(line);
		public static string GetEnv(string env) => Environment.GetEnvironmentVariable(env);
		#endregion
	}
}
