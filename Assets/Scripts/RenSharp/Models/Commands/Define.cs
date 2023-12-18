using RenSharp.Core;

namespace RenSharp.Models.Commands
{
	internal class Define : Command
	{
		public string PythonLine { get; set; }

		public Define(string pythonLine)
		{
			PythonLine = pythonLine;
		}

		public override void Execute(RenSharpCore core)
		{
			// Ignore at runtime
		}

		public void ExecuteDefine(RenSharpCore core)
		{
			// Нужно превести команду к Define, чтобы увидеть этот метод
			core.Context.ExecutePython(PythonLine);
		}
	}
}
