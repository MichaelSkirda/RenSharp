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

        public void Goto(Label label) => Goto(label.Line);
        public void Goto(string label) => Goto(GetLabel(label).Line);
        public void Goto(int line)
        {
            // -1 because line to index
            // -1 because we call MoveNext() before get command
            Position = line - 2;
        }

        public Label GetLabel(string name)
        {
            var labels = _program
                .OfType<Label>()
                .Where(x => x.Name == name);

            if (labels.Count() > 1)
                throw new ArgumentException($"There are two labels with name {name}");

            Label label = labels.First();

            if (label == null)
                throw new NullReferenceException($"Null label was found by name {name}.");

            return label;
        }

        public void Reset() => Goto("main");
		public void Dispose() { }
    }
}
