using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenSharp
{
	internal class RenSharpProgram : IEnumerator<Command>
	{
		private List<Label> CachedLabels { get; set; } = new List<Label>();

		object IEnumerator.Current => Current;
		private int Position = -1;
		private List<Command> _program { get; set; }
		internal IReadOnlyList<Command> Code
		{
			get
			{
				return _program.AsReadOnly();
			}
		}
		public Command Current
		{
			get
			{
				return _program[Position];
			}
		}

		public RenSharpProgram(List<Command> program)
		{
			_program = program;
			SetupProgram();
		}

		private void SetupProgram()
		{
			CacheLabels();

			Goto("main");
		}

		private void CacheLabels()
		{
			IEnumerable<Label> labelDefines = _program
				.OfType<Label>();

			foreach (var labelDefine in labelDefines)
			{
				Label label = GetLabel(labelDefine.Name);
				CachedLabels.Add(label);
			}
		}

		public bool MoveNext()
		{
			if (Position < _program.Count - 1)
			{
				Position++;
				return true;
			}
			else
				return false;
		}

		public void Goto(string label) => Goto(GetLabel(label).Line);
		public void Goto(int line)
		{
			Position = line - 1;
		}

		public Label GetLabel(string name)
		{
			Label label = CachedLabels.FirstOrDefault(x => x.Name == name);
			if(label != null)
				return label;
			
			label = _program
				.OfType<Label>()
				.FirstOrDefault(x => x.Name == name);
			if (label == null)
				return null;

			label.Line = _program.IndexOf(label) + 1;
			CachedLabels.Add(label);
			return label;
		}

		public void Reset()
		{
			Label main = _program.OfType<Label>().FirstOrDefault(x => x.Name == "main");
			Position = _program.IndexOf(main);
		}

		public void Dispose()
		{

		}
	}
}
