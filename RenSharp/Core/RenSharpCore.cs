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
        private RenSharpContext Context { get; set; }

        public RenSharpCore(string path, Configuration config = null) => SetupProgram(File.ReadAllLines(path), config);
        public RenSharpCore(IEnumerable<string> code, Configuration config = null) => SetupProgram(code, config);
        private void SetupProgram(IEnumerable<string> code, Configuration config)
        {
            Context = new RenSharpContext();
			if (config == null)
            {
				config = new Configuration();
                config.UseDefault();
			}
			config.UseCoreCommands();

			Configuration = config;
            RenSharpReader reader = new RenSharpReader(config);

			var program = reader.ParseCode(code.ToList());
			Program = new RenSharpProgram(program);
		}

        public Command ReadNext()
        {
            Command command;
			bool skip;

			do
			{
                skip = false;
                bool hasNext = Program.MoveNext();
                if (!hasNext)
                    throw new Exception("Unexpected end of game");

                command = Program.Current;
                if (command.Level > Context.Level)
                {
                    skip = true;
					continue;
				}
				else
                    Context.Level = command.Level;

				command.Execute(this, Context);
			}
			while (Configuration.IsSkip(command) || skip);

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
