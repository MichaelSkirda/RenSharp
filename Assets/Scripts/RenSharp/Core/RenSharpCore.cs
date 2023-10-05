﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RenSharp.Core.Exceptions;
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
		private bool HasStarted { get; set; } = false;

        public RenSharpCore(string path, Configuration config = null) => SetupProgram(File.ReadAllLines(path), config);
        public RenSharpCore(IEnumerable<string> code, Configuration config = null) => SetupProgram(code, config);
		public RenSharpCore(Configuration config = null) => SetupProgram(config);

		private void SetupProgram(IEnumerable<string> code, Configuration config)
		{
			SetupProgram(config);
			LoadProgram(code);
		}
		private void SetupProgram(Configuration config)
        {
			PyImportAttribute.ReloadCallbacks();
			Context = new RenSharpContext();

			if (config == null)
				config = DefaultConfiguration.GetDefaultConfig();

            Writer = config.Writer;
			Configuration = config;
		}

		public void LoadProgram(string path, bool saveScope = false)
			=> LoadProgram(File.ReadAllLines(path), saveScope);
        public void LoadProgram(IEnumerable<string> code, bool saveScope = false)
        {
			if (saveScope == false)
				Context.RecreateScope();

			RenSharpReader reader = new RenSharpReader(Configuration);
			var program = reader.ParseCode(code);

			Context.Program = new RenSharpProgram(program);
			HasStarted = false;
		}

		/// <summary>
		/// Запускает все блоки init и переводит курсор на блок 'start'
		/// </summary>
		/// <exception cref="Exception">Если label 'start' не найден</exception>
		/// <exception cref="ArgumentException">Если label 'start' имеет табы</exception>
		public void Start()
		{
			if (HasStarted)
				return;
			HasStarted = true;
			ExecuteInits();

			Label main = Program.GetLabel("start");
			if (main == null)
				throw new Exception("Программа обязана иметь точку входа start");
			if (main.Level != 1)
				throw new ArgumentException("Лейбл 'start' не должен иметь табуляции");
			Context.Goto(main);
		}

        public Command ReadNext()
        {
			if (!HasStarted)
				Start();

            Command command;
			bool skip;

			do
			{
                skip = false;
                bool hasNext = Program.MoveNext();
                if (!hasNext)
                    throw new UnexpectedEndOfProgramException("Неожиданный конец программы.");

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

        private void ExecuteInits()
        {
			List<Init> inits = Context.Program.Code
				.OfType<Init>()
				.OrderByDescending(x => x.Priority)
				.ThenBy(x => x.Line)
				.ToList();

			foreach (Init init in inits)
			{
				Command initStart = Program[init.Line + 1];
				Context.Goto(initStart);
				Command command;

				while (true)
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
		}
    }
}
