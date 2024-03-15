using Newtonsoft.Json;
using RenSharp.Core.Exceptions;
using RenSharp.Core.Parse;
using RenSharp.Core.Repositories;
using RenSharp.Core.Save;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharp.Models.Commands;
using RenSharp.Models.Save;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace RenSharp.Core
{
    public class RenSharpCore
	{
		public bool IsPaused { get; private set; } = false;
		public Mutex Mutex { get; private set; } = new Mutex();

		public Configuration Configuration { get; set; }
		public RenSharpContext Context { get; set; }
		private RenSharpProgram Program => Context.Program;
		private bool HasStarted { get; set; } = false;
		private CharacterRepository CharacterRepository { get; set; }

        public RenSharpCore(string path, Configuration config = null) => SetupProgram(File.ReadAllLines(path), config);
        public RenSharpCore(IEnumerable<string> code, Configuration config = null) => SetupProgram(code, config);
		public RenSharpCore(Configuration config = null)
		{
			SetConfig(config);
            Context = new RenSharpContext();
            SetupProgram();
		}

		private void SetupProgram(IEnumerable<string> code, Configuration config)
		{
			SetConfig(config);
            Context = new RenSharpContext();
            LoadProgram(code, saveScope: true);
			SetupProgram();
        }

		private void SetupProgram()
		{
            CharacterRepository = new CharacterRepository();
            string key = CharacterRepository.AddCharacter(new Character(name: "???"));

            PyImportAttribute.ReloadCallbacks();
            Context.SetVariable("rs", this);
            Context.SetVariable("_rs_nobody_character", key);
        }

		private void SetConfig(Configuration config)
		{
            if (config == null)
                Configuration = DefaultConfiguration.GetDefaultConfig();
            else
                Configuration = config;
        }

		public SaveModel SaveRaw()
		{
			var save = new SaveModel()
			{
				IsPaused = IsPaused,
				HasStarted = HasStarted,
				Line = Program.Line,
				MessageHistory = Context.MessageHistory.Messages,
				CallStack = Context.CallStack,
				RollbackStack = Context.RollbackStack,
				CurrentFrame = Context.CurrentFrame,
				Variables = Context.PyEvaluator.GetVariables()
			};
			return save;
		}

		public string SaveJson()
		{
			SaveModel save = SaveRaw();
			string serialized = JsonConvert.SerializeObject(save);
			return serialized;
		}

		public void Load(string serializedSave)
		{
			SaveModelJson savePreParsed = JsonConvert.DeserializeObject<SaveModelJson>(serializedSave);
			IEnumerable<Type> commandTypes = Assembly
				.GetAssembly(typeof(Command))
				.GetTypes()
				.Where(x => x.IsSubclassOf(typeof(Command)));

			List<Command> RollbackStack = new List<Command>();

			foreach(object commandJObject in savePreParsed.RollbackStack)
			{
				string commandJson = commandJObject.ToString();
				string commandTypeName = JsonConvert.DeserializeObject<CommandTypeJson>(commandJson).TypeName;

				Func<string, RenSharpCore, Command> parser;
                bool hasParser = Configuration.DeserializeParsers.TryGetValue(commandTypeName, out parser);


                if (hasParser == false || parser == null)
					throw new ArgumentException($"По значению '{commandTypeName}' указан не валидный парсер.");

				Command command = null;

				try
				{
                    command = parser(commandJson, this);
                }
				catch
				{
					throw new ArgumentException("Can not parse command from save.");
                }

                if (command == null)
					throw new ArgumentException("Can not parse command from save.");
				else
				{
                    RollbackStack.Add(command);
					continue;
                }

            }

			var save = new SaveModel(savePreParsed, RollbackStack);
            Load(save);
        }

		public void Load(SaveModel save)
		{
			SetConfig(Configuration);
			SetupProgram();
			Context.PyEvaluator.RecreateScope();

			// If save should be start
			if (save.HasStarted)
			{
				HasStarted = false;
				Start();
			}

			Program.Goto(save.Line);
			IsPaused = save.IsPaused;

			Context.Load(save);
		}

		public string AddCharacter(string name)
		{
			var character = new Character(name);
			string key = CharacterRepository.AddCharacter(character);
			return key;
		}

		public bool Rollback()
		{
			Command command;

			while(true)
			{
				bool hasRollback = Context.RollbackStack.TryPop(out command);
				if (hasRollback == false)
					return false;

				Goto(command);

				command.Execute(this);

				if (Configuration.IsNotSkip(command))
					break;
			}
			return true;
		}

		public void LoadProgram(string path, bool saveScope = false)
			=> LoadProgram(File.ReadAllLines(path), saveScope);
        public void LoadProgram(IEnumerable<string> code, bool saveScope = false)
        {
			var parser = new RenSharpParser(Configuration);
			var program = parser.ParseCode(code);

			LoadProgram(program, saveScope);
		}

		public void LoadProgram(List<Command> program, bool saveScope = false)
		{
			if (saveScope == false)
			{
                Context.PyEvaluator.RecreateScope();
                SetupProgram();
            }

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
            try
			{
				bool isMutexFree = Mutex.WaitOne(millisecondsTimeout: 1000);
				if (isMutexFree == false)
					throw new RenSharpLockedException("RenSharp заблокирован. Скорее всего идет выполнение другой команды.");

                if (!HasStarted)
                    Start();
                if (IsPaused && force == false)
                    throw new RenSharpPausedException("RenSharp находится на паузе.");

                Command command = ExecuteNext();
                return command;
            }
            catch { throw; }
			finally
			{
				Mutex.ReleaseMutex();
			}
        }

		private Command ExecuteNext()
		{
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

				if(command is IRollbackable)
				{
                    Command backwardCommand = (command as IRollbackable)?.Rollback(this);
                    if (backwardCommand != null)
                        Context.RollbackStack.Push(backwardCommand);
                }
                

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
			Character character;
            try
			{
				string systemName = Context.GetVariable<string>(characterName);
				bool exists = CharacterRepository.TryGetCharacter(systemName, out character);
				if (exists == false)
					throw new InvalidOperationException();
				if (character == null)
					throw new InvalidOperationException();

				return character.Attributes;
			}
			catch
			{
				throw new InvalidOperationException($"Персонаж с именем '{characterName}' не найден.");
			}
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
			IEnumerable<Init> inits = Context.Program.Code
				.OfType<Init>()
				.OrderByDescending(x => x.Priority)
				.ThenBy(x => x.Line);

			IEnumerable<Define> defines = Context.Program.Code
				.OfType<Define>();

			foreach(Define define in defines)
			{
				define.ExecuteDefine(this);
			}

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
