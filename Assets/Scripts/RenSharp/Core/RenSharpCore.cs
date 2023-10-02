using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharp.Models.Commands;

namespace RenSharp.Core
{
    public class RenSharpCore
    {
        private RenSharpProgram Program => Context.Program;
        public Configuration Configuration { get; set; }
        public IWriter Writer { get; set; }
        private RenSharpContext Context { get; set; }

        public RenSharpCore(string path, Configuration config = null) => SetupProgram(File.ReadAllLines(path), config);
        public RenSharpCore(IEnumerable<string> code, Configuration config = null) => SetupProgram(code, config);
        private void SetupProgram(IEnumerable<string> code, Configuration config)
        {
			CallbackAttribute.ReloadCallbacks();
			Context = new RenSharpContext();

			if (config == null)
				config = new Configuration();

            config.UseDefault().UseCoreCommands();
            Writer = config.Writer;

			Configuration = config;

            RenSharpReader reader = new RenSharpReader(config);
			var program = reader.ParseCode(code);

			Context.Program = new RenSharpProgram(program);

            List<Init> inits = Context.Program.Code
                .OfType<Init>()
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.Line)
                .ToList();

            foreach(Init init in inits)
            {
                Command initStart = Program[init.Line + 1];
                Context.Goto(initStart);
				Command command;

				while(true)
				{
					bool hasNext = Program.MoveNext();
                    if (!hasNext)
                        break;

					command = Program.Current;
					if (command.Level > Context.Level)
						continue;

					int cycleStart = 0;
					while (command.Level < Context.Level)
					{
						cycleStart = Context.LevelStack.Pop();
						if (cycleStart != 0)
							break;
					}
					if (cycleStart != 0)
					{
						Program.Goto(cycleStart);
						continue;
					}

					if (command.Level <= 1)
						break;

					command.Execute(this, Context);
				}
			}

            Label main = Program.GetLabel("main");
            if (main.Level != 1)
                throw new ArgumentException("Main must have no tabulation.");
            Context.Goto(main);
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

                int cycleStart = 0;
				while (command.Level < Context.Level)
                {
                    cycleStart = Context.LevelStack.Pop();
                    if (cycleStart != 0)
                        break;
				}
                if (cycleStart != 0)
                {
                    Program.Goto(cycleStart);
                    skip = true;
                    continue;
                }

                command.Execute(this, Context);
			} while (Configuration.IsSkip(command) || skip);

            return command;
        }

        public void Goto(string labelName) => Goto(Program.GetLabel(labelName));
        public void Goto(Command command) => Context.Goto(command);

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
