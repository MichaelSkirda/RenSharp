using System;
using System.Collections.Generic;
using System.IO;
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

        public RenSharpCore(string path, Configuration config = null) => SetupProgram(File.ReadAllLines(path), config);
        public RenSharpCore(IEnumerable<string> code, Configuration config = null) => SetupProgram(code, config);
        private void SetupProgram(IEnumerable<string> code, Configuration config)
        {
			if (config == null)
            {
				config = new Configuration();
                config.UseDefault();
			}
            Configuration = config;
            RenSharpReader reader = new RenSharpReader(config);

			var program = reader.ParseCode(code.ToList());
			Program = new RenSharpProgram(program);
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
        public Attributes GetCharacterAttributes(string characterName)
        {
            Character character = Program.Code
                .OfType<Character>()
                .FirstOrDefault(x => x.Name == characterName);

            if (character == null)
                return new Attributes(new string[0]);

            return character.Attributes;
        }
    }
}
