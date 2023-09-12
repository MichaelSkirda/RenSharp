using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharp.Models.Commands;

namespace RenSharp.Core
{
    public class RenSharpCore
    {
        private RenSharpProgram Program;
        public Configuration Configuration { get; set; }
        public IWriter Writer { get; set; }

        public RenSharpCore(string path)
        {
            var program = RenSharpReader.ParseCode(path);
            Program = new RenSharpProgram(program);
            Configuration = new Configuration();
            Configuration.UseDefaultSkips();
        }

        public RenSharpCore(List<string> code)
        {
            var program = RenSharpReader.ParseCode(code);
            Program = new RenSharpProgram(program);
            Configuration = new Configuration();
            Configuration.UseDefaultSkips();
        }

        public Command ReadNext()
        {
            Command command;

            do
            {
                bool hasNext = Program.MoveNext();
                if (!hasNext)
                    throw new Exception("Unexpected end of game");

                command = Program.Current;
                command.Execute(this);
            }
            while (Configuration.IsSkip(command));

            return command;
        }

        public void GotoLabel(string labelName) => Program.Goto(labelName);
        public List<string> GetCharacterAttributes(string characterName)
        {
            Character character = Program.Code
                .OfType<Character>()
                .FirstOrDefault(x => x.Name == characterName);

            if (character == null)
                return new List<string>();

            return character.Styles;
        }
    }
}
