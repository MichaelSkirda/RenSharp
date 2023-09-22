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

        public void Goto(Label label) => Goto(label.Line);
        public void Goto(string label) => Goto(GetLabel(label).Line);
        public void Goto(int line)
        {
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
                return null;

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
