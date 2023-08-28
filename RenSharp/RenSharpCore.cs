using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RenSharp.Models;
using RenSharp.Models.Commands;

namespace RenSharp
{
	internal class RenSharpCore
	{
		private RenSharpProgram Program;
		public Configuration Configuration { get; set; }

		public RenSharpCore(List<string> code)
		{
			Program = new RenSharpProgram(RenSharpReader.ParseCode(code));
			Configuration = new Configuration();
			Configuration.UseDefaultSkips();
		}

		public void ReadNext()
		{
			Command command = null;

			do
			{
				bool hasNext = Program.MoveNext();
				if (!hasNext)
					throw new Exception("Unexpected end of game");

				command = Program.Current;
				command.Execute(this);
			}
			while (Configuration.IsSkip(command));
		}

		public void GotoLabel(string name)
		{
			Label label = GetLabel(name);
			Program.Goto(label);
		}

		public Label GetLabel(string name)
		{
			Label label = Program.GetLabel(name);
			if (label == null)
				throw new ArgumentException($"Label '{name}' not found!");

			return label;
		}
	}
}
