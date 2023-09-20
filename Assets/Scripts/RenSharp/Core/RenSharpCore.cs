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

                int cycleStart = 0;
				while (command.Level < Context.Level)
                {
                    cycleStart = Context.Stack.Pop();
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
			Context.Stack.Clear();
			while (Context.Level < label.Level)
				Context.Stack.Push(0);
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

        public void Callback(string funcName, RenSharpCore renSharpCore, string[] args)
        {
			CallbackAttribute.CallMethod(funcName, renSharpCore, args);
		}

        public void RegisterCallback(string name, MethodInfo method, object obj = null)
        {
            CallbackAttribute.RegisterMethod(name, method, obj);
        }

        public void Goto(int line) => Program.Goto(line);
        
    }
}
