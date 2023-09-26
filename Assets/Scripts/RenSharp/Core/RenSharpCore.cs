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
			CallbackAttribute.Init();
			Context = new RenSharpContext();

			if (config == null)
				config = new Configuration().UseDefault().UseCoreCommands();
            
			Configuration = config;

            RenSharpReader reader = new RenSharpReader(config);
			var program = reader.ParseCode(code);

			Context.Program = new RenSharpProgram(program);

            List<Init> inits = Context.Program.Code
                .OfType<Init>()
                .ToList();

            foreach(Init init in inits)
            {
                Program.Goto(init.Line + 1);
				Command command;
                bool skip;

				do
				{
					skip = false;
					bool hasNext = Program.MoveNext();
                    if (!hasNext)
                        break;

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

					if (command.Level <= 1)
						break;

					command.Execute(this, Context);
				} while (Configuration.IsSkip(command) || skip);
			}

            Program.Goto("main");
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

        public void GotoLabel(string labelName)
        {
            Label label = Program.GetLabel(labelName);
            GotoClearStack(label);
            Program.Goto(label);
        }
        
        private void GotoClearStack(Label label)
        {
			Context.LevelStack.Clear();
			while (Context.Level < label.Level)
				Context.LevelStack.Push(0);
		}

        public Attributes GetCharacterAttributes(string characterName)
        {
            Character character = Program.Code
                .OfType<Character>()
                .FirstOrDefault(x => x.Name == characterName);

            if (character == null)
                return new Attributes(new string[0]);

            return character.Attributes;
        }

        public void RegisterCallback(string name, MethodInfo method)
        {
            CallbackAttribute.RegisterMethod(name, method);
        }
    }
}
