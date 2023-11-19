﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.RenSharp.Core.Exceptions;
using RenSharp.Core.Exceptions;
using RenSharp.Core.Parse;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharp.Models.Commands;

namespace RenSharp.Core
{
	public class RenSharpCore
	{
		public bool IsPaused { get; private set; } = false;
		public Configuration Configuration { get; set; }
		public RenSharpContext Context { get; set; }
		private RenSharpProgram Program => Context.Program;
		private RenSharpParser Parser { get; set; }
		private bool HasStarted { get; set; } = false;

        public RenSharpCore(string path, Configuration config = null) => SetupProgram(File.ReadAllLines(path), config);
        public RenSharpCore(IEnumerable<string> code, Configuration config = null) => SetupProgram(code, config);
		public RenSharpCore(Configuration config = null) => SetupProgram(config);

		private void SetupProgram(IEnumerable<string> code, Configuration config)
		{
			SetupProgram(config);
			LoadProgram(code, saveScope: true);
		}

		private void SetupProgram(Configuration config)
        {
			if (config == null)
				config = DefaultConfiguration.GetDefaultConfig();

			PyImportAttribute.ReloadCallbacks();
			Configuration = config;
			Context = new RenSharpContext();
			Context.SetVariable("rs", this);
			Parser = new RenSharpParser(Configuration);
		}

		public void Rollback()
		{
			Command command;
			bool isFirst = true;

			while(true)
			{
				bool hasRollback = Context.RollbackStack.TryPop(out command);
				if (hasRollback == false)
					throw new InvalidOperationException("Rollback пуст");

				Goto(command);

				if(command.GetType() == typeof(Message) && isFirst && false)
				{
					isFirst = false;
					continue;
				}
				isFirst = false;

				command.Execute(this);

				if (Configuration.IsNotSkip(command))
					break;
			}
		}

		public void LoadProgram(string path, bool saveScope = false)
			=> LoadProgram(File.ReadAllLines(path), saveScope);
        public void LoadProgram(IEnumerable<string> code, bool saveScope = false)
        {
			if (saveScope == false)
				Context.PyEvaluator.RecreateScope();

			var program = Parser.ParseCode(code);

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
			Goto(main);
		}

        public Command ReadNext(bool force = false)
        {
			if (!HasStarted)
				Start();
			if (IsPaused && force == false)
				throw new RenSharpPausedException("RenSharp находится на паузе.");

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

				Command backwardCommand = command.Rollback(this);
				if(backwardCommand != null)
					Context.RollbackStack.Push(backwardCommand);

                command.Execute(this);
				(command as IPushable)?.TryPush(Context);
			} while (Configuration.IsSkip(command) || skip);

            return command;
        }

		public void ClearRollback()
		{
			Context.ClearRollback();
			Context.MessageHistory.Clear();
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

		public object GetVariable(string name)
			=> Context.GetVariable(name);
		public T GetVariable<T>(string name)
			=> Context.GetVariable<T>(name);

		public void SetVariable(string name, object value)
			=> Context.SetVariable(name, value);

		public void Pause() => IsPaused = true;
		public void Resume() => IsPaused = false;

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
				Goto(initStart);
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

					command.Execute(this);
				}
			}
		}
    }
}
