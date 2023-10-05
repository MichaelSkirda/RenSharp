using RenSharp.Core.Exceptions;
using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenSharp.Core
{
    internal class RenSharpProgram : IEnumerator<Command>
    {
        private int Position = -1;
        private List<Command> _program { get; set; }
		internal Command this[int line] => _program[line - 1];
		internal IReadOnlyList<Command> Code => _program.AsReadOnly();
        public Command Current => _program[Position];
        

        internal RenSharpProgram(List<Command> program)
        {
            _program = program;
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

		internal void Goto(int line)
        {
			// -1 because line to index
			int index = line - 1;
            // -1 because we call MoveNext() before get command
            Position = index - 1;
        }

		internal Label GetLabel(string name)
        {
            var labels = _program
                .OfType<Label>()
                .Where(x => x.Name == name);

            if (labels.Count() > 1)
                throw new ArgumentException($"There are two labels with name {name}");

            Label label = labels.FirstOrDefault();

            if (label == null)
                throw new LabelNotExists($"Лейбл с именем '{name}' не существует.");

            return label;
        }

        public void Reset() => throw new NotImplementedException();
		public void Dispose() { }
		object IEnumerator.Current => Current;
	}
}
